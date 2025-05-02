/* Formatting function for row details */
function format(data) {
    let races = data.races || [];
    let html = '<div>' +
               '<h6>Заезды:</h6>';

    if (races.length === 0) {
        html += '<p>Нет заездов для этого заказа.</p></div>';
        return html;
    }

    html += '<table class="cell-border hover stripe" style="width:auto; border-collapse:collapse;">' +
            '<thead>' +
            '<tr>' +
            '<th style="padding:8px;">ID</th>' +
            '<th style="padding:8px;">Дата</th>' +
            '<th style="padding:8px;">Время</th>' +
            '<th style="padding:8px;">Категория</th>' +
            '<th style="padding:8px;">Статус</th>' +
            '</tr>' +
            '</thead>' +
            '<tbody>';

    races.forEach(race => {
        // Extract date and times from startDate and finishDate
        const startDateTime = new Date(race.startDate);
        const finishDateTime = new Date(race.finishDate);
        const date = startDateTime.toISOString().split('T')[0]; // yyyy-MM-dd
        const startTime = startDateTime.toTimeString().slice(0, 5); // HH:mm
        const finishTime = finishDateTime.toTimeString().slice(0, 5); // HH:mm
        const timeRange = `${startTime} - ${finishTime}`;

        html += '<tr>' +
                '<td style="padding:8px;">' + race.id + '</td>' +
                '<td style="padding:8px;">' + date + '</td>' +
                '<td style="padding:8px;">' + timeRange + '</td>' +
                '<td style="padding:8px;">' + race.raceCategory + '</td>' +
                '<td style="padding:8px;">' + race.raceStatus + '</td>' +
                '</tr>';
    });

    html += '</tbody></table></div>';
    return html;
}

var ordersTable = new DataTable('#adminOrdersMain', {
    responsive: true,
    scrollX: true,
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
    order: [[2, 'desc']] // Order by Order Date
});

// Add event listener for opening and closing details
ordersTable.on('click', 'td.dt-control', function (e) {
    let tr = e.target.closest('tr');
    let row = ordersTable.row(tr);

    if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.classList.remove('shown');
    } else {
        // Open this row
        let rowData = {
            races: JSON.parse(tr.getAttribute('data-races') || '[]')
        };
        row.child(format(rowData)).show();
        tr.classList.add('shown');
    }
});

// Orders select fields
$(document).ready(function() {
    $('#reg-user').select2({
        placeholder: "Выберите пользователя...",
        allowClear: true,
        width: '100%'
    });

    /* Maintain ASP.NET Core validation */
    $('#reg-user').on('change', function() {
        $(this).valid(); /* Trigger validation */
    });
});