
function updateRaceRadioButtons(slots, containerId){
    var container = document.getElementById(containerId);
    while (container.hasChildNodes()) {
        container.removeChild(container.lastChild);
    }

    var RaceTypeRadios = document.getElementsByName('race-type');
    var checkValue = 'None';
    RaceTypeRadios.forEach(button => {
        if (button.checked){
            checkValue = button.value;
        }
    });
    switch (checkValue){
        case 'uniform':  // Uniform
            addRaceRadioButtonsDivided(slots, container, 0);
            break;
        case 'divided':  // Divided
            addRaceRadioButtonsDivided(slots, container, 1);
            break;
    }
}

function addRaceRadioButtonsDivided(slots, container, mode=1) {

    /*<div class="form-check form-check-inline">
    <input class="form-check-input" type="radio" name="race-type" id="raceTypeRadioUniform" value="option1">
    <label class="form-check-label" for="RaceTypeRadioUniform">Один тип заездов для всех слотов</label>
    </div>
    <div class="form-check form-check-inline">
    <input class="form-check-input" type="radio" name="race-type" id="raceTypeRadioDivided" value="option2">
    <label class="form-check-label" for="RaceTypeRadioDivided">Различные типы заездов</label>

    <label for="staticEmail" class="col-sm-2 col-form-label">Email</label>
    </div>*/
    var rowAmount = slots.length;
    if (!mode){
        rowAmount = 1;
    }
    console.log(rowAmount);
    for (var rowCount = 0; rowCount < rowAmount; rowCount++){
        // <div class="form-group row"></div>
        var rowContainer = document.createElement('div');
        rowContainer.classList.add('row');
        container.appendChild(rowContainer);
        
        var titleContainer = document.createElement('h5');
        if (mode){
            titleContainer.textContent = `Слот ${slots[rowCount]}`;
        }
        else{
            titleContainer.textContent = `Выберите тип заезда:`;
        }
        rowContainer.appendChild(titleContainer);

        var inputGroupContainer = document.createElement('div');
        inputGroupContainer.classList.add('input-group');
        rowContainer.appendChild(inputGroupContainer);

        const radioOptions = ['Взрослый', 'Детский', 'Семейный'];

        for (var radioCount = 0; radioCount < radioOptions.length; radioCount++){
            var colThirdContainer = document.createElement('div');
            colThirdContainer.classList.add('col-third');

            var inputContainer = document.createElement('input');
            var inputContainerId = `raceTypeRadio_${slots[rowCount]}_${radioCount}`;
            inputContainer.type = 'radio';
            inputContainer.name = `race-type_${slots[rowCount]}`;
            inputContainer.id = inputContainerId
            inputContainer.value = `raceTypeRadio_${slots[rowCount]}_Option_${radioCount}`;
            //<input id="race-type-divided" type="radio" name="race-type" value="divided">

            var labelContainer = document.createElement('label');
            labelContainer.htmlFor = inputContainerId;
            var labelContainerText = document.createTextNode(radioOptions[radioCount]);
            labelContainer.appendChild(labelContainerText);
            //<label for="race-type-divided">Различные типы заездов</label>

            colThirdContainer.appendChild(inputContainer);
            colThirdContainer.appendChild(labelContainer);

            inputGroupContainer.appendChild(colThirdContainer);
        }
        /*// <label for="" class="col-sm-2 col-form-label">Text</label>
        var label = document.createElement('label');
        label.htmlFor = '';
        label.classList.add('col-sm-2', 'col-form-label');
        if (mode){
            var labeltext = document.createTextNode(`Слот ${slots[rowCount]}:`);
        }
        else{
            var labeltext = document.createTextNode(`Выберите тип заезда:`);
        }

        label.appendChild(labeltext);
        rowContainer.appendChild(label);

        // <div class="col-sm-10">
        var radioContainerGlobal = document.createElement('div');
        radioContainerGlobal.classList.add('col-sm-10');

        */
        
        //const radioOptions = ['Взрослый', 'Детский', 'Семейный'];

        /*for (var radioCount = 0; radioCount < radioOptions.length; radioCount++){
            // <div class="form-check form-check-inline">
            var radioContainer1 = document.createElement('div');
            radioContainer1.classList.add('form-check', 'form-check-inline');

            // <input class="form-check-input" type="radio" name="race-type0" id="raceTypeRadio0_0" value="raceTypeRadio0Option0">
            var inputContainer1 = document.createElement('input');
            var inputContainer1Id = `raceTypeRadio_${slots[rowCount]}_${radioCount}`;
            inputContainer1.classList.add('form-check-input');
            inputContainer1.type = 'radio';
            inputContainer1.name = `race-type_${slots[rowCount]}`;
            inputContainer1.id = inputContainer1Id
            inputContainer1.value = `raceTypeRadio_${slots[rowCount]}_Option_${radioCount}`;

            // <label class="form-check-label" for="RaceTypeRadioDivided">Различные типы заездов</label>
            var labelContainer = document.createElement('label');
            labelContainer.classList.add('form-check-label');
            labelContainer.htmlFor = inputContainer1Id;
            var labelContainerText = document.createTextNode(radioOptions[radioCount]);
            labelContainer.appendChild(labelContainerText);

            radioContainer1.appendChild(inputContainer1);
            radioContainer1.appendChild(labelContainer);

            radioContainerGlobal.appendChild(radioContainer1);
        }*/

        //rowContainer.appendChild(radioContainerGlobal);

        //container.appendChild(document.createElement('br'));
    }
}

function checkSelectedSlots(slots) {

    var raceTypeSelection = document.getElementById('RaceTypeGlobalContainer');
    /*if (slots.length == 0){
        raceTypeSelection.style.visibility = 'visible';
    }
    else{
        raceTypeSelection.style.visibility = 'visible';
    }*/
}


window.onload = function() {
    var timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
        interval: 15,
        startTime: 10,
        endTime: 20,
        title: 'Свободные слоты',
        static: false,
        useSlotTemplate : false

    });


    document.getElementById('inputtime').addEventListener('change.appo.picker', function (e) {
        var slots = timePicker.getTimes();
        console.log(slots);
        updateRaceRadioButtons(slots, 'RaceTypeContainer');
        checkSelectedSlots(slots);
    })

    var datePicker = new AirDatepicker(document.getElementById('inputdate'), {
        dateFormat(date) {
            return date.toLocaleString('ru', {
                year: 'numeric',
                day: '2-digit',
                month: 'long'
            });
        },

        onSelect({date, formattedDate, datepicker}){
            date.setHours(date.getHours() + 5);
            const newdate = new Date(date);
            const promise = axios({
                method: 'get',
                url: '/api/appointments/day',
                params: {
                    querydate: date,
                }
            })
            .then(function (response) {
                var disable = [];
                //console.log(data);
                response.data.forEach(elem => {
                    disable.push(elem);
                });
                console.log(disable);
                timePicker.destroy();
                timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
                    startTime: 10,
                    endTime: 20,
                    disabled: disable,
                    title: 'Свободные слоты',
                    static: false,
                    useSlotTemplate : false
                });
                var slots = timePicker.getTimes();
                //updateRaceRadioButtons(slots, 'RaceTypeContainer');
                checkSelectedSlots(slots);
            })
            .catch(function (error) {
                // handle error
                console.log(error);
            });


        }
    })

    var RaceTypeRadios = document.getElementsByName('race-type');
    RaceTypeRadios.forEach(button => {
        button.addEventListener('change', function(){
            updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer', button.value)
        });
        //button.onclick = updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer', button.value);
    });

    
    function postForm(event){
        var form = document.getElementById('orderForm')
        var data = new FormData(form);
    
        var date = datePicker.selectedDates;
        if (date.length == 0) return;
    
        //data.set('time', timePicker.getTimes())
        
        for (var [key, value] of data.entries()) { 
            //console.log(key, value);
          }
          
        const request = axios({
            method: "post",
            url: "api/appointments/order",
            data: data,
          })
            .then(function (response) {
              //handle success
              console.log('/');
              console.log(response);
              window.location.href = '/Index';
            })
            .catch(function (response) {
              //handle error
              console.log('//');
              console.log(response);
            });
    }

    document.getElementById('submitForm').onclick = postForm;
}