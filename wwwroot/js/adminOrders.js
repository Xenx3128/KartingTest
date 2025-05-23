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
        // Parse startDate and finishDate, assuming UTC
        const startDateTime = new Date(race.startDate + 'Z'); // Append 'Z' to treat as UTC
        const finishDateTime = new Date(race.finishDate + 'Z');

        // Adjust to UTC+05:00 by adding 5 hours (5 * 60 * 60 * 1000 milliseconds)
        const utcOffsetMs = 5 * 60 * 60 * 1000;
        const startDateTimeUtcPlus5 = new Date(startDateTime.getTime() + utcOffsetMs);
        const finishDateTimeUtcPlus5 = new Date(finishDateTime.getTime() + utcOffsetMs);

        // Extract date and times
        const date = startDateTimeUtcPlus5.toISOString().split('T')[0]; // yyyy-MM-dd
        const startTime = startDateTimeUtcPlus5.toISOString().slice(11, 16); // HH:mm
        const finishTime = finishDateTimeUtcPlus5.toISOString().slice(11, 16); // HH:mm
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

$(document).ready(function() {
    var ordersTable = new DataTable('#adminOrdersMain', {
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
        dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Excel',
                title: 'Заказы',
                exportOptions: {
                    columns: [2, 3, 4, 5] // Export only visible data columns (exclude control and ID)
                }
            },
            {
                extend: 'csvHtml5',
                text: 'CSV',
                title: 'Заказы',
                exportOptions: {
                    columns: [2, 3, 4, 5] // Export only visible data columns
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                title: 'Заказы',
                exportOptions: {
                    columns: [2, 3, 4, 5] // Export only visible data columns
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
                targets: -1, // Actions column
                orderable: false,
                searchable: false
            }
        ],
        
        order: [[2, 'desc']],
        initComplete: function () {
            this.api().columns([1, 2, 3, 4, 5]).every(function () {
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
    $('#reg-user').select2({
        placeholder: "Выберите пользователя...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-user').on('change', function() {
        $(this).valid(); /* Trigger validation */
    });

    // Handle delete button click to show modal
    $('.delete-btn').on('click', function() {
        const orderId = $(this).data('order-id');
        const modal = $(`#delete-modal-${orderId}`);
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
        const orderId = $(this).data('order-id');
        const form = $(`.delete-form .delete-btn[data-order-id="${orderId}"]`).closest('form');
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
