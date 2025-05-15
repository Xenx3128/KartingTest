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
            dom: '<"dt-top"<"dt-length"l><"dt-right"<"dt-buttons"B><"dt-search"f>>>rtip',
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    title: 'Карты',
                    exportOptions: {
                        columns: [1, 2] // Export only visible data columns (exclude control and ID)
                    }
                },
                {
                    extend: 'csvHtml5',
                    text: 'CSV',
                    title: 'Карты',
                    exportOptions: {
                        columns: [1, 2] // Export only visible data columns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    title: 'Карты',
                    exportOptions: {
                        columns: [1, 2] // Export only visible data columns
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

    // Handle delete button click to show modal
    $('.delete-btn').on('click', function() {
        const cartId = $(this).data('cart-id');
        const modal = $(`#delete-modal-${cartId}`);
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
        const cartId = $(this).data('cart-id');
        const form = $(`.delete-form .delete-btn[data-cart-id="${cartId}"]`).closest('form');
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
