import { initializePickers } from './appointment.js';

$(document).ready(function() {
    // Initialize Select2 for dropdowns
    if (typeof $.fn.select2 !== 'undefined') {
        $('#reg-status').select2({
            placeholder: "Выберите статус...",
            allowClear: true,
            width: '100%'
        });

        $('#reg-status').on('change', function() {
            $(this).valid();
        });
    } else {
        console.warn('Select2 not loaded, falling back to native select');
    }

    // Initialize Appointment Picker for create/edit pages
    if ($('#breakcreateform').length || $('#breakeditform').length) {
        const isEditPage = window.location.pathname.includes('/Edit');
        const startTime = isEditPage ? window.breakStartTime : '';
        console.log('Initializing pickers with isEditPage:', isEditPage, 'startTime:', startTime);
        initializePickers(isEditPage, startTime);
    }

    if ($('#adminBreaksMain').length) {
        // Initialize DataTable
        const table = new DataTable('#adminBreaksMain', {
            responsive: true,
            language: {
                info: 'Заезд _START_-_END_ из _TOTAL_',
                infoEmpty: 'Тут пока пусто',
                infoFiltered: '(отфильтровано из _MAX_ заездов)',
                lengthMenu: 'Показывать _MENU_ заездов на странице',
                search: "Поиск:",
                zeroRecords: 'Ничего не найдено - извините'
            },
            columnDefs: [
                {
                    targets: -1, // Actions column
                    orderable: false,
                    searchable: false
                }
            ],
            order: [[1, 'desc']],
            initComplete: function () {
                this.api().columns([0, 1, 2, 3, 4]).every(function () {
                    let column = this;
                    let footer = $(column.footer()).html('<input type="text" placeholder="Фильтр..." style="width:100%"/>');
                    
                    $('input', column.footer()).on('keyup change clear', function () {
                        if (column.search() !== this.value) {
                            column.search(this.value).draw();
                        }
                    });
                });
            }
        });
    }
});