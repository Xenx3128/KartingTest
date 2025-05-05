/* Formatting function for row details */
function format(data) {
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
            '<td><strong>Откуда узнали:</strong></td>' +
            '<td>' + (data.fromWhereFoundOut || 'N/A') + '</td>' +
        '</tr>' +
        '<tr>' +
            '<td><strong>Примечание:</strong></td>' +
            '<td>' + (data.note || 'N/A') + '</td>' +
        '</tr>' +
        '</table>'
    );
}

var usertable = new DataTable('#adminUsersMain', {
    responsive: true,
    scrollX: true,
    language: {
        info: 'Пользователь _START_-_END_ из _TOTAL_',
        infoEmpty: 'Тут пока пусто',
        infoFiltered: '(отфильтровано из _MAX_ пользователей)',
        lengthMenu: 'Показывать _MENU_ пользователей на странице',
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
            targets: 1, // ID column
            visible: false
        }
    ],
    order: [[2, 'asc']],
    initComplete: function () {
        this.api().columns([2, 3, 4, 5, 6, 7, 8]).every(function () {
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

// Add event listener for opening and closing details
usertable.on('click', 'td.dt-control', function (e) {
    let tr = e.target.closest('tr');
    let row = usertable.row(tr);

    if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.classList.remove('shown');
    } else {
        // Open this row
        let rowData = {
            fromWhereFoundOut: tr.getAttribute('data-from-where'),
            note: tr.getAttribute('data-note')
        };
        row.child(format(rowData)).show();
        tr.classList.add('shown');
    }
});
