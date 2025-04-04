
    $(document).ready(function () {
        LoadStudent();
    });

    function LoadStudent() {
        $.ajax({
            url: '/Student/GetStudentList',
            type: 'GET',
            dataType: 'json',  // 'json' should be lowercase
            success: function (data) {
                let row = '';
                $.each(data, function (index, student) {
                    row += `<tr>
                        <td>${student.id}</td>
                        <td>${student.name}</td>
                        <td>${student.email}</td>
                        <td>${student.class}</td>
                        <td>${student.age}</td>
                        <td>${student.isactive ? 'Active' : 'Inactive'}</td>
                        <td>
                            <button class="btn btn-primary btn-sm" onclick="EditStudent(${student.id})">Edit</button>
                            <button class="btn btn-danger btn-sm" onclick="DeleteStudent(${student.id})">Delete</button>
                        </td>
                    </tr>`;
                });
                $('#userTable tbody').html(row); // Ensure you target <tbody>
            },
            error: function () {
                alert("Failed to load users.");
            }
        });
    }

