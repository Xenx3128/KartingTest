import { initializePickers } from './appointment.js';

$(document).ready(function() {
    $('#reg-order, #reg-category, #reg-status').select2({
        placeholder: "Выберите...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-order, #reg-category, #reg-status').on('change', function() {
        $(this).valid();
    });

    // Initialize Appointment Picker for create/edit pages
    if ($('#raceEditForm').length) {
        const isEditPage = window.location.pathname.includes('/Edit');
        const startTime = isEditPage ? window.raceStartTime : '';
        console.log('Initializing pickers with isEditPage:', isEditPage, 'startTime:', startTime);
        initializePickers(isEditPage, startTime);
    }

    // Initialize DataTable for races list
    if ($('#adminRacesMain').length) {
        new DataTable('#adminRacesMain', {
            responsive: true,
            language: {
                info: 'Заезд _START_-_END_ из _TOTAL_',
                infoEmpty: 'Тут пока пусто',
                infoFiltered: '(отфильтровано из _MAX_ заездов)',
                lengthMenu: 'Показывать _MENU_ заездов на странице',
                search: "Поиск:",
                zeroRecords: 'Ничего не найдено - извините'
            },
            dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    title: 'Заезды',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5] // Export only visible data columns (exclude control and ID)
                    }
                },
                {
                    extend: 'csvHtml5',
                    text: 'CSV',
                    title: 'Заезды',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5] // Export only visible data columns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    title: 'Заезды',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5] // Export only visible data columns
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
                this.api().columns([0, 1, 2, 3, 4, 5]).every(function () {
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
        const raceId = $(this).data('race-id');
        const modal = $(`#delete-modal-${raceId}`);
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
        const raceId = $(this).data('race-id');
        const form = $(`.delete-form .delete-btn[data-race-id="${raceId}"]`).closest('form');
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
