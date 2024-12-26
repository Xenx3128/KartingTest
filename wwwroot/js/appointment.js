
function updateRaceRadioButtons(slots, containerId){
    console.log(checkValue);
    var container = document.getElementById(containerId);
    while (container.hasChildNodes()) {
        container.removeChild(container.lastChild);
    }

    var RaceTypeRadios = document.getElementsByName('raceTypeRadioOptions');
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
    <input class="form-check-input" type="radio" name="raceTypeRadioOptions" id="raceTypeRadioUniform" value="option1">
    <label class="form-check-label" for="RaceTypeRadioUniform">Один тип заездов для всех слотов</label>
    </div>
    <div class="form-check form-check-inline">
    <input class="form-check-input" type="radio" name="raceTypeRadioOptions" id="raceTypeRadioDivided" value="option2">
    <label class="form-check-label" for="RaceTypeRadioDivided">Различные типы заездов</label>

    <label for="staticEmail" class="col-sm-2 col-form-label">Email</label>
    </div>*/
    var rowAmount = slots.length;
    if (!mode){
        rowAmount = 1;
    }

    for (var rowCount = 0; rowCount < rowAmount; rowCount++){
        // <div class="form-group row"></div>
        var rowContainer = document.createElement('div');
        rowContainer.classList.add('form-group', 'row');
        container.appendChild(rowContainer);
        
        // <label for="" class="col-sm-2 col-form-label">Text</label>
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


        
        const radioOptions = ['Взрослый', 'Детский', 'Семейный'];

        for (var radioCount = 0; radioCount < radioOptions.length; radioCount++){
            // <div class="form-check form-check-inline">
            var radioContainer1 = document.createElement('div');
            radioContainer1.classList.add('form-check', 'form-check-inline');

            // <input class="form-check-input" type="radio" name="raceTypeRadioOptions0" id="raceTypeRadio0_0" value="raceTypeRadio0Option0">
            var radioInput1 = document.createElement('input');
            var radioInput1Id = `raceTypeRadio_${slots[rowCount]}_${radioCount}`;
            radioInput1.classList.add('form-check-input');
            radioInput1.type = 'radio';
            radioInput1.name = `raceTypeRadioOptions_${slots[rowCount]}`;
            radioInput1.id = radioInput1Id
            radioInput1.value = `raceTypeRadio_${slots[rowCount]}_Option_${radioCount}`;

            // <label class="form-check-label" for="RaceTypeRadioDivided">Различные типы заездов</label>
            var radioLabel1 = document.createElement('label');
            radioLabel1.classList.add('form-check-label');
            radioLabel1.htmlFor = radioInput1Id;
            var radioLabel1Text = document.createTextNode(radioOptions[radioCount]);
            radioLabel1.appendChild(radioLabel1Text);

            radioContainer1.appendChild(radioInput1);
            radioContainer1.appendChild(radioLabel1);

            radioContainerGlobal.appendChild(radioContainer1);
        }

        rowContainer.appendChild(radioContainerGlobal);

        //container.appendChild(document.createElement('br'));
    }
}

function checkSelectedSlots(slots) {

    var raceTypeSelection = document.getElementById('RaceTypeGlobalContainer');
    if (slots.length == 0){
        raceTypeSelection.style.visibility = 'hidden';
    }
    else{
        raceTypeSelection.style.visibility = 'visible';
    }
}


window.onload = function() {
    var timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
        interval: 15,
        startTime: 10,
        endTime: 18,
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
                    disable.push(elem.slice(0, -3));
                });
                console.log(timePicker);
                timePicker.destroy();
                timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
                    interval: 15,
                    startTime: 10,
                    endTime: 18,
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

    var RaceTypeRadios = document.getElementsByName('raceTypeRadioOptions');
    RaceTypeRadios.forEach(button => {
        button.addEventListener('change', function(){
            updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer', button.value)
        });
        //button.onclick = updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer', button.value);
    });

    document.getElementById('submitForm').onclick = postForm;

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
              console.log(response);
            })
            .catch(function (response) {
              //handle error
              console.log(response);
            });
    }
}