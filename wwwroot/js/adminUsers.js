/* Formatting function for row details */
function format(data) {
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
            '<td><strong>Откуда узнали:</strong></td>' +
            '<td>' + (data.fromWhereFoundOut || 'Нет') + '</td>' +
        '</tr>' +
        '<tr>' +
            '<td><strong>Примечание:</strong></td>' +
            '<td>' + (data.note || 'Нет') + '</td>' +
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
    dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
    buttons: [
        {
            extend: 'excelHtml5',
            text: 'Excel',
            title: 'Пользователи',
            exportOptions: {
                columns: [2, 3, 4, 5, 6, 7, 8] // Export only visible data columns (exclude control and ID)
            }
        },
        {
            extend: 'csvHtml5',
            text: 'CSV',
            title: 'Пользователи',
            exportOptions: {
                columns: [2, 3, 4, 5, 6, 7, 8] // Export only visible data columns
            }
        },
        {
            extend: 'pdfHtml5',
            text: 'PDF',
            title: 'Пользователи',
            exportOptions: {
                columns: [2, 3, 4, 5, 6, 7, 8] // Export only visible data columns
            }
        },
    ],
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

document.addEventListener('DOMContentLoaded', function () {
    // Handle delete button click to show modal
    document.querySelectorAll('.delete-btn').forEach(button => {
        button.addEventListener('click', function () {
            const userId = this.getAttribute('data-user-id');
            const modal = document.getElementById(`delete-modal-${userId}`);
            if (modal) {
                modal.classList.add('active');
            }
        });
    });

    // Handle modal close button
    document.querySelectorAll('.modal__close').forEach(button => {
        button.addEventListener('click', function () {
            const modal = this.closest('.modal');
            if (modal) {
                modal.classList.remove('active');
            }
        });
    });

    // Handle confirm delete button
    document.querySelectorAll('.confirm-delete').forEach(button => {
        button.addEventListener('click', function () {
            const userId = this.getAttribute('data-user-id');
            const form = document.querySelector(`form.delete-form [data-user-id="${userId}"]`).closest('form');
            if (form) {
                form.submit(); // Submit the form programmatically
            }
        });
    });

    // Close modal when clicking outside the modal content
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', function (e) {
            if (e.target === modal) {
                modal.classList.remove('active');
            }
        });
    });
});