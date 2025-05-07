
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
});


