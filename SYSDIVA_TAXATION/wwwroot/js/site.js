$(document).ready(function () {
    LoadUsers();

    });
//--------------Load All users--------
    function LoadUsers() {
        $.ajax({
            url: '/User/GetUsers',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                let row = '';
                $.each(data, function (index, user) {
                    row += `<tr>
                     <td>${user.id}</td>
                    <td>${user.name}</td>
                    <td>${user.email}</td>
                    <td>${user.age}</td>
                    <td>${user.salary}</td>
                    <td>${user.gender}</td>
                    <td>${new Date(user.createdOn).toLocaleDateString()}</td>
                    <td>${user.isActive ? 'Active' : 'Inactive'}</td>
                     <td>
                            <button class="btn btn-primary btn-sm" onclick="EditStudent(${user.id})">Edit</button>
                            <button class="btn btn-danger btn-sm" onclick="DeleteUser('+user.id+');">Delete</button>
                        </td>
                </tr>`;
                });
                $('#userTable tbody').html(row); // Ensure tbody is targeted
            },
            error: function () {
                alert("Failed to load users.");
            }
        });
    }
    //-------------------------------------insert-------------
    $('#btnAddEmployee').click(function () {
        $('#ModalEmployee').modal('show');
    })
//--------
function AddUser() {
    debugger;

    var createdOn = $('#CreatedOn').val();
    if (!createdOn) {
        createdOn = new Date().toISOString(); // Set current date if empty
    }

    var object = {
        Name: $('#Name').val(),
        Email: $('#Email').val(),
        Age: parseInt($('#Age').val()) || 0,  
        Salary: parseFloat($('#Salary').val()) || 0.0,  
        Gender: $('#Gender').val(),
        CreatedOn: createdOn,  // Ensure proper date format
        IsActive: $('#isActive').is(':checked')  // Convert checkbox to boolean
    };

    $.ajax({
        url: '/User/Insert/',
        type: 'POST',
        data: JSON.stringify(object), // Convert object to JSON
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            alert('Data saved successfully');
            LoadUsers();
            HideModelPopUp()
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('Data could not be saved');
        }
    });
}
function HideModelPopUp() {
    $('#ModalEmployee').modal('hide');
}
// Delete User

function DeleteUser(id) {
    if (!id) {
        alert("Invalid User ID!");
        return;
    }

    if (confirm("Are you sure you want to delete this user?")) {
        $.ajax({
            url: '/User/UserDelete', // No ID in URL
            type: 'POST', // Use POST instead of GET
            data: { id: id }, // Send ID in request body
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    alert("Data deleted successfully.");
                    loadUsers(); // Refresh user list (Ensure this function exists)
                } else {
                    alert("Failed to delete data: " + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                alert("Failed to delete data.");
            }
        });
    }
}














    // Export Users all data  to Excel
    function downloadExcel() {
        window.location.href = "/User/ExportToExcel";
    }
// -------------------searching start-----------------------


$(document).ready(function () {
    $("#searchBox").on("keyup", function () {
        let value = $(this).val().toLowerCase();

        $("#userTable tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
});

    // -----