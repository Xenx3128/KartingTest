let timePicker;
let datePicker;
let raceCategories = [];

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

export async function fetchRaceCategories() {
    try {
        const response = await axios.get('/api/appointments/categories');
        raceCategories = response.data;
        console.log('Fetched race categories:', raceCategories);
        return raceCategories;
    } catch (error) {
        console.error('Error fetching race categories:', error);
        return [];
    }
}


export async function initializePickers(isEditPage = false, initialTime='') {
    const settings = await fetchSelectedSettings();
    if (isEditPage){
        timePicker = new AppointmentPicker(document.getElementById('inputtime'), {
            interval: settings.raceDuration,
            startTime: settings.dayStart,
            endTime: settings.dayFinish,
            title: 'Свободные слоты',
            static: false,
            useSlotTemplate: false
        });
        timePicker.setTime(initialTime);
    }
    else {
        timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
            interval: settings.raceDuration,
            startTime: settings.dayStart,
            endTime: settings.dayFinish,
            title: 'Свободные слоты',
            static: false,
            useSlotTemplate: false
        });
    }


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
                if (isEditPage){
                    timePicker = new AppointmentPicker(document.getElementById('inputtime'), {
                        interval: settings.raceDuration,
                        startTime: settings.dayStart,
                        endTime: settings.dayFinish,
                        disabled: disabledTimes,
                        title: 'Свободные слоты',
                        static: false,
                        useSlotTemplate: false
                    });
                    timePicker.setTime(initialTime);
                }
                else {
                    timePicker = new AppointmentSlotPicker(document.getElementById('inputtime'), {
                        interval: settings.raceDuration,
                        startTime: settings.dayStart,
                        endTime: settings.dayFinish,
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

export function getTimePicker() {
    return timePicker;
}

export function getDatePicker() {
    return datePicker;
}

export class DateOnly {
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

export class TimeOnly {
    static fromTimeString(timeString) {
        if (typeof timeString !== 'string' || !/^\d{2}:\d{2}$/.test(timeString)) {
            throw new Error('Invalid time format. Expected HH:mm');
        }
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