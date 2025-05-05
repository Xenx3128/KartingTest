$(document).ready(function() {
    // Initialize Select2 for dropdowns
    $('#reg-status').select2({
        placeholder: "Выберите...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-status').on('change', function() {
        $(this).valid();
    });
    
    // Initialize DataTable for carts list
    if ($('#adminCartsMain').length) {
        new DataTable('#adminCartsMain', {
            responsive: true,
            language: {
                info: 'Картинг _START_-_END_ из _TOTAL_',
                infoEmpty: 'Тут пока пусто',
                infoFiltered: '(отфильтровано из _MAX_ картингов)',
                lengthMenu: 'Показывать _MENU_ картингов на странице',
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
                this.api().columns([0, 1, 2]).every(function () {
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