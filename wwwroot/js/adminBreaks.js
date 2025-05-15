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
                info: 'Перерыв _START_-_END_ из _TOTAL_',
                infoEmpty: 'Тут пока пусто',
                infoFiltered: '(отфильтровано из _MAX_ перерывов)',
                lengthMenu: 'Показывать _MENU_ перерывов на странице',
                search: "Поиск:",
                zeroRecords: 'Ничего не найдено - извините'
            },
            dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    title: 'Тех перерывы',
                    exportOptions: {
                        columns: [1, 2, 3, 4] // Export only visible data columns (exclude control and ID)
                    }
                },
                {
                    extend: 'csvHtml5',
                    text: 'CSV',
                    title: 'Тех перерывы',
                    exportOptions: {
                        columns: [1, 2, 3, 4] // Export only visible data columns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    title: 'Тех перерывы',
                    exportOptions: {
                        columns: [1, 2, 3, 4] // Export only visible data columns
                    }
                },
            ],
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

    // Handle delete button click to show modal
    $('.delete-btn').on('click', function() {
        const breakId = $(this).data('break-id');
        const modal = $(`#delete-modal-${breakId}`);
        if (modal.length) {
            modal.addClass('active');
        }
    });

    // Handle modal close button
    $('.modal__close').on('click', function() {
        const modal = $(this).closest('.modal');
        if (modal.length) {
            modal.removeClass('active');
        }
    });

    // Handle confirm delete button
    $('.confirm-delete').on('click', function() {
        const breakId = $(this).data('break-id');
        const form = $(`.delete-form .delete-btn[data-break-id="${breakId}"]`).closest('form');
        if (form.length) {
            form[0].submit(); // Submit the form programmatically
        }
    });

    // Close modal when clicking outside the modal content
    $('.modal').on('click', function(e) {
        if (e.target === this) {
            $(this).removeClass('active');
        }
    });
});
