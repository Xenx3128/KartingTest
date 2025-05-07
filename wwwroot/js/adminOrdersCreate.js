let timePicker;
let datePicker;
let raceCategories = [];

async function fetchRaceCategories() {
    try {
        const response = await axios.get('/api/appointments/categories');
        raceCategories = response.data;
    } catch (error) {
        console.error('Error fetching race categories:', error);
    }
}
async function fetchSelectedSettings() {
    try {
        const response = await axios.get('/api/settings/selected');
        console.log('Fetched settings:', response.data);
        return response.data;
    } catch (error) {
        console.error('Error fetching selected settings:', error);
        return {
            dayStart: 10,
            dayFinish: 20,
            raceDuration: 15
        };
    }
}


function updateRaceRadioButtons(slots, containerId) {
    const container = document.getElementById(containerId);
    container.innerHTML = ''; // Clear existing content

    const raceTypeRadios = document.getElementsByName('race-type');
    const isUniform = Array.from(raceTypeRadios).find(r => r.checked)?.value === 'uniform';
    
    const rowCount = isUniform ? 1 : slots.length;
    
    for (let i = 0; i < rowCount; i++) {
        const rowContainer = document.createElement('div');
        rowContainer.classList.add('row', 'mt-2');
        container.appendChild(rowContainer);

        const title = document.createElement('h5');
        title.className = 'reg-label';
        title.textContent = isUniform ? 'Выберите тип заезда:' : `Слот ${slots[i]}:`;
        rowContainer.appendChild(title);

        const inputGroup = document.createElement('div');
        inputGroup.classList.add('input-group');
        rowContainer.appendChild(inputGroup);

        raceCategories.forEach((category, index) => {
            const col = document.createElement('div');
            col.classList.add('col-third');

            const input = document.createElement('input');
            input.type = 'radio';
            input.name = isUniform ? 'Input.RaceCategoryIds[0]' : `Input.RaceCategoryIds[${i}]`;
            input.id = `race-category-${i}-${index}`;
            input.value = category.id;
            input.classList.add('form-check-input');
            input.required = true;

            const label = document.createElement('label');
            label.htmlFor = input.id;
            label.classList.add('reg-label');
            label.textContent = category.category;

            col.appendChild(input);
            col.appendChild(label);
            inputGroup.appendChild(col);
        });
    }
}

async function initializePickers() {
    const settings = await fetchSelectedSettings();

    timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
        interval: settings.raceDuration,
        startTime: settings.dayStart,
        endTime: settings.dayFinish,
        title: 'Свободные слоты',
        static: false,
        useSlotTemplate: false
    });

    datePicker = new AirDatepicker(document.getElementById('inputdate'), {
        dateFormat: 'dd MMMM yyyy',
        minDate: new Date(),
        onSelect: async ({ date }) => {
            if (!date) return;
            try {
                const selectedDate = DateOnly.fromDate(date).toString();
                const response = await axios.get('/api/appointments/day', {
                    params: { querydate: selectedDate }
                });
                // Filter disabled times for the selected date
                const disabledTimes = response.data
                    .filter(item => item.date === selectedDate)
                    .map(item => item.time);
                console.log('Disabled times for', selectedDate, ':', disabledTimes); // Debug
                timePicker.destroy();
                timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
                    interval: settings.raceDuration,
                    startTime: settings.dayStart,
                    endTime: settings.dayFinish,
                    disabled: disabledTimes, // Pass HH:mm strings
                    title: 'Свободные слоты',
                    static: false,
                    useSlotTemplate: false
                });
                updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer');
            } catch (error) {
                console.error('Error fetching unavailable times:', error);
            }
        }
    });
}

document.addEventListener('DOMContentLoaded', async () => {
    // Initialize Select2 for user dropdown
    $('#reg-user').select2({
        placeholder: "Выберите пользователя...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-user').on('change', function() {
        $(this).valid(); // Trigger validation
    });

    await fetchRaceCategories();
    initializePickers();

    document.getElementById('inputtime').addEventListener('change.appo.picker', () => {
        const times = timePicker.getTimes();
        console.log('Selected times:', times); // Debug
        updateRaceRadioButtons(times, 'RaceTypeContainer');
    });

    document.getElementsByName('race-type').forEach(radio => {
        radio.addEventListener('change', () => {
            updateRaceRadioButtons(timePicker.getTimes(), 'RaceTypeContainer');
        });
    });

    document.getElementById('orderForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const form = e.target;

        const date = datePicker.selectedDates[0];
        if (!date) {
            alert('Выберите дату');
            return;
        }

        const times = timePicker.getTimes();
        if (!times || times.length === 0) {
            alert('Выберите хотя бы одно время');
            return;
        }

        const formData = new FormData(form);
        const isUniform = formData.get('race-type') === 'uniform';
        const raceCategoryIds = isUniform
            ? [parseInt(formData.get('Input.RaceCategoryIds[0]'))]
            : times.map((_, i) => parseInt(formData.get(`Input.RaceCategoryIds[${i}]`)));

        if (raceCategoryIds.some(id => isNaN(id))) {
            alert('Выберите категорию для каждого слота');
            return;
        }

        if (!formData.get('terms')) {
            alert('Необходимо принять технику безопасности');
            return;
        }

        // Validate and format times as HH:mm
        const formattedTimes = times
            .map(t => {
                try {
                    const timeObj = TimeOnly.fromTimeString(t);
                    return timeObj.toString();
                } catch (error) {
                    console.error('Invalid time value:', t, error);
                    return null;
                }
            })
            .filter(t => t !== null);

        if (formattedTimes.length === 0) {
            alert('Неверный формат времени. Пожалуйста, выберите корректные временные слоты.');
            return;
        }

        // Populate hidden inputs for form submission
        const dateInput = document.createElement('input');
        dateInput.type = 'hidden';
        dateInput.name = 'Input.Date';
        dateInput.value = DateOnly.fromDate(date).toString();
        form.appendChild(dateInput);

        formattedTimes.forEach((time, i) => {
            const timeInput = document.createElement('input');
            timeInput.type = 'hidden';
            timeInput.name = `Input.Times[${i}]`;
            timeInput.value = time;
            form.appendChild(timeInput);
        });

        raceCategoryIds.forEach((id, i) => {
            const categoryInput = document.createElement('input');
            categoryInput.type = 'hidden';
            categoryInput.name = `Input.RaceCategoryIds[${i}]`;
            categoryInput.value = id;
            form.appendChild(categoryInput);
        });

        const isUniformInput = document.createElement('input');
        isUniformInput.type = 'hidden';
        isUniformInput.name = 'Input.IsUniform';
        isUniformInput.value = isUniform.toString();
        form.appendChild(isUniformInput);

        const termsInput = document.createElement('input');
        termsInput.type = 'hidden';
        termsInput.name = 'Input.TermsAccepted';
        termsInput.value = 'true';
        form.appendChild(termsInput);

        console.log('Submitting form with data:', {
            userId: formData.get('Input.UserId'),
            date: dateInput.value,
            times: formattedTimes,
            isUniform: isUniform,
            raceCategoryIds: raceCategoryIds,
            termsAccepted: true
        }); // Debug

        form.submit();
    });
});

// Helper classes for DateOnly and TimeOnly serialization
class DateOnly {
    static fromDate(date) {
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            throw new Error('Invalid date');
        }
        return {
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            day: date.getDate(),
            toString: function() {
                return `${this.year}-${this.month.toString().padStart(2, '0')}-${this.day.toString().padStart(2, '0')}`;
            }
        };
    }
}

class TimeOnly {
    static fromTimeString(timeString) {

        const [hour, minute] = timeString.split(':').map(Number);
        if (hour < 0 || hour > 23 || minute < 0 || minute > 59) {
            throw new Error('Invalid time values');
        }
        return {
            hour,
            minute,
            toString: function() {
                return `${this.hour.toString().padStart(2, '0')}:${this.minute.toString().padStart(2, '0')}`;
            }
        };
    }

    static fromDate(date) {
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            throw new Error('Invalid time');
        }
        return {
            hour: date.getHours(),
            minute: date.getMinutes(),
            toString: function() {
                return `${this.hour.toString().padStart(2, '0')}:${this.minute.toString().padStart(2, '0')}`;
            }
        };
    }
}