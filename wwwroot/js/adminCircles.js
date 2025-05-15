$(document).ready(function() {
    var circlesTable = new DataTable('#adminCircleResultsMain', {
        responsive: true,
        scrollX: true,
        language: {
            info: 'Круг _START_-_END_ из _TOTAL_',
            infoEmpty: 'Тут пока пусто',
            infoFiltered: '(отфильтровано из _MAX_ кругов)',
            lengthMenu: 'Показывать _MENU_ кругов на странице',
            search: "Поиск:",
            zeroRecords: 'Ничего не найдено - извините'
        },
        dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Excel',
                title: 'Круги',
                exportOptions: {
                    columns: [0, 1] // Export only visible data columns (exclude control and ID)
                }
            },
            {
                extend: 'csvHtml5',
                text: 'CSV',
                title: 'Круги',
                exportOptions: {
                    columns: [0, 1] // Export only visible data columns
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                title: 'Круги',
                exportOptions: {
                    columns: [0, 1] // Export only visible data columns
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
        order: [[2, 'desc']],
    });

    // Handle delete button click to show modal
    $('.delete-btn').on('click', function() {
        const resultId = $(this).data('result-id');
        const modal = $(`#delete-modal-${resultId}`);
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
        const resultId = $(this).data('result-id');
        const form = $(`.delete-form .delete-btn[data-result-id="${resultId}"]`).closest('form');
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
