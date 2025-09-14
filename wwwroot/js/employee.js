$(document).ready(function () {
    debugger;
    var table = $('#employeeTable').DataTable({
        processing: true,
        ajax: {
            url: '/Employee/GetAllEmployees',
            dataSrc: ''
        },
        columns: [
            { data: 'empId' },
            { data: 'empName' },
            { data: 'empEmail' },
            { data: 'empDepartment' },
            { data: 'empSalary' },
            {
                data: 'empId',
                render: function (data) {
                    return `
<button class="btn btn-sm btn-primary edit-btn" data-id="${data}">Edit</button>
<button class="btn btn-sm btn-danger delete-btn" data-id="${data}">Delete</button>
`;
                },
                orderable: false
            }
        ]
    });


     //show empty modal for Add
    $('#addEmployeeBtn').on('click', function () {
        clearModal();
        $('#employeeModalLabel').text('Add Employee');
        var modal = new bootstrap.Modal(document.getElementById('employeeModal'));
        modal.show();
    });


    // Edit
    
    $('#employeeTable').on('click', '.edit-btn', function () {
        var id = $(this).data('id');  // must exist in button

        $.get(`/Employee/GetEmployeeById/${id}`, function (data) {
            $('#EmpId').val(data.empId);
            $('#EmpName').val(data.empName);
            $('#Email').val(data.empEmail);
            $('#Department').val(data.empDepartment);
            $('#Salary').val(data.empSalary);

            $('#employeeModalLabel').text('Edit Employee');
            var modal = new bootstrap.Modal(document.getElementById('employeeModal'));
            modal.show();
        }).fail(function (xhr) {
            console.error("Error:", xhr);
            showAlert('Error loading employee (maybe wrong route or id missing)', 'danger');
        });
    });



    $('#employeeTable').on('click', '.delete-btn', function () {
        debugger;
        var id = $(this).data('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "This record will be deleted!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Employee/DeleteEmployee/' + id, // send in route
                    type: 'POST',
                   /* data: { id: id },*/
                    success: function (response) {
                        if (response.success) {
                            Swal.fire('Deleted!', 'Employee has been deleted.', 'success');
                            $('#employeeTable').DataTable().ajax.reload();
                        } else {
                            Swal.fire('Error!', 'Failed to delete employee.', 'error');
                        }
                    },
                    error: function () {
                        Swal.fire('Error!', 'Something went wrong.', 'error');
                    }
                });
            }
        });
    });

    $('#employeeForm').validate({

        rules: {
            EmpName: {
                required: true,
                maxlength: 100
            },
            Email: {
                required: true,
                email: true,
                remote: {
                    url: '/Employee/CheckEmail',
                    type: 'POST',
                    data: {
                        email: function () {
                            return $('#Email').val();
                        },
                        empId: function () {   // pass EmpId to backend
                            return $('#EmpId').val();
                        }
                    }
                }
            },
            Department: {
                required: true
            },
            Salary: {
                required: true,
                number: true,
                min: 1
            }
        },
        messages: {
            EmpName: {
                required: "Employee name is required",
                maxlength: "Name cannot exceed 100 characters"
            },
            Email: {
                required: "Email is required",
                email: "Enter a valid email address",
                remote: "This email already exists"
            },
            Department: {
                required: "Department is required"
            },
            Salary: {
                required: "Salary is required",
                number: "Enter a valid number",
                min: "Salary cannot be negative or 0"
            }
        },

        errorClass: "text-danger",
        errorElement: "span",
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },

        submitHandler: function (form) {
           
            var empId = parseInt($('#EmpId').val()) || 0;

            var employee = {
                empId: empId,
                empName: $('#EmpName').val(),
                empEmail: $('#Email').val(),
                empDepartment: $('#Department').val(),
                empSalary: parseFloat($('#Salary').val()) || 0
            };

            var url = empId > 0 ? '/Employee/UpdateEmployee' : '/Employee/SaveEmployee';
            var actionText = empId && empId > 0 ? 'updated' : 'saved';

            $.ajax({
                url: url,
                method: 'POST',
                contentType: "application/json",
                data: JSON.stringify(employee),
                success: function (res) {
                    if (res.success) {
                        Swal.fire({
                            title: 'Success!',
                            text: 'Employee ' + actionText + ' successfully',
                            icon: 'success',
                            confirmButtonText: 'OK'
                        }).then(() => {
                            var modal = bootstrap.Modal.getInstance(document.getElementById('employeeModal'));
                            modal.hide();
                            table.ajax.reload();  // refresh DataTable
                        });
                    } else {
                        Swal.fire({
                            title: 'Failed!',
                            text: res.message || 'Operation failed',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                },
                error: function (xhr) {
                    Swal.fire({
                        title: 'Error!',
                        text: xhr.responseText || 'Error during operation',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            });
        }

    });






    


    //$('#employeeModal').on('hidden.bs.modal', function () {
    //    var form = $('#employeeForm');

    //    // Reset form fields
    //    form[0].reset();

    //    // Clear validation error messages and styles
    //    form.validate().resetForm();
    //    form.find('.is-invalid').removeClass('is-invalid');
    //});




    function clearModal() {
        $('#EmpId').val(0);
        $('#EmpName').val('');
        $('#Email').val('');
        $('#Department').val('');
        $('#Salary').val('');
        $('#employeeForm').validate().resetForm();
    }


    function showAlert(message, type) {
        var html = `<div class="alert alert-${type} alert-dismissible fade show" role="alert">${message}
<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
</div>`;
        $('#alertPlaceholder').html(html);
    }
});