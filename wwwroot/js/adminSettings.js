$(document).ready(function() {
    $('#reg-settings').select2({
        placeholder: "Выберите настройки...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-settings').on('change', function() {
        $(this).valid();
    });

    var settingsTable = new DataTable('#adminSettingsMain, #adminSettingsChoose', {
        responsive: true,
        scrollX: true,
        language: {
            info: 'Настройки _START_-_END_ из _TOTAL_',
            infoEmpty: 'Тут пока пусто',
            infoFiltered: '(отфильтровано из _MAX_ настроек)',
            lengthMenu: 'Показывать _MENU_ настроек на странице',
            search: "Поиск:",
            zeroRecords: 'Ничего не найдено - извините'
        },
        dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Excel',
                title: 'Настройки',
                exportOptions: {
                    columns: [1, 2, 3, 4] // Export only visible data columns (exclude control and ID)
                }
            },
            {
                extend: 'csvHtml5',
                text: 'CSV',
                title: 'Настройки',
                exportOptions: {
                    columns: [1, 2, 3, 4] // Export only visible data columns
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                title: 'Настройки',
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
        order: [[0, 'asc']],
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

    // Handle delete button click to show modal
    $('.delete-btn').on('click', function() {
        const settingsId = $(this).data('settings-id');
        const modal = $(`#delete-modal-${settingsId}`);
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
        const settingsId = $(this).data('settings-id');
        const form = $(`.delete-form .delete-btn[data-settings-id="${settingsId}"]`).closest('form');
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
