$(document).ready(function() {

    var circlesTable = new DataTable('#adminCircleResultsMain', {
        responsive: true,
        scrollX: true,
        language: {
            info: 'Заказ _START_-_END_ из _TOTAL_',
            infoEmpty: 'Тут пока пусто',
            infoFiltered: '(отфильтровано из _MAX_ заказов)',
            lengthMenu: 'Показывать _MENU_ заказов на странице',
            search: "Поиск:",
            zeroRecords: 'Ничего не найдено - извините'
        },
        columnDefs: [
            {
                className: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: '',
                targets: 0
            },
            {
                targets: -1, // Actions column
                orderable: false,
                searchable: false
            }
        ],
        order: [[2, 'desc']],
    });
});

