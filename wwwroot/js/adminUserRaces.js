import { initializePickers } from './appointment.js';

$(document).ready(function() {
    // Initialize Select2 for dropdowns
    $('#reg-user, #reg-cart').select2({
        placeholder: "Выберите...",
        allowClear: true,
        width: '100%'
    });

    $('#reg-user, #reg-cart').on('change', function() {
        $(this).valid();
    });

    // Initialize DataTable for user races list
    if ($('#adminUserRacesMain').length) {
        new DataTable('#adminUserRacesMain', {
            responsive: true,
            language: {
                info: 'Участник _START_-_END_ из _TOTAL_',
                infoEmpty: 'Тут пока пусто',
                infoFiltered: '(отфильтровано из _MAX_ участников)',
                lengthMenu: 'Показывать _MENU_ участников на странице',
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
            order: [[0, 'asc']] // Order by ID column
        });
    }
});