let timePicker;
let datePicker;

export function initializePickers(isEditPage = false, initialTime='') {
    if (isEditPage){
        timePicker = new AppointmentPicker(document.getElementById('inputtime'), {
            interval: 15,
            startTime: 10,
            endTime: 20,
            title: 'Свободные слоты',
            static: false,
            useSlotTemplate: false
        });
        timePicker.setTime(initialTime);
    }
    else {
        timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
            interval: 15,
            startTime: 10,
            endTime: 20,
            title: 'Свободные слоты',
            static: false,
            useSlotTemplate: false
        });
    }


    datePicker = new AirDatepicker(document.getElementById('inputdate'), {
        dateFormat: 'dd MMMM yyyy',
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
                if (isEditPage){
                    timePicker = new AppointmentPicker(document.getElementById('inputtime'), {
                        interval: 15,
                        startTime: 10,
                        endTime: 20,
                        disabled: disabledTimes,
                        title: 'Свободные слоты',
                        static: false,
                        useSlotTemplate: false
                    });
                    timePicker.setTime(initialTime);
                }
                else {
                    timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
                        interval: 15,
                        startTime: 10,
                        endTime: 20,
                        disabled: disabledTimes, // Pass HH:mm strings
                        title: 'Свободные слоты',
                        static: false,
                        useSlotTemplate: false
                    });
                }
            } catch (error) {
                console.error('Error fetching unavailable times:', error);
            }
        }
    });
}

// Helper class for DateOnly serialization
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