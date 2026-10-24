
let students = [];

document.getElementById("studentForm").addEventListener("submit", function(event) {
    event.preventDefault();

    let name = document.getElementById("name").value.trim();
    let dept = document.getElementById("dept").value.trim();
    let reg = document.getElementById("reg").value;

    let student = {
        name: name,
        dept: dept,
        reg: reg
    };

    students.push(student);

    displayStudents();

    document.getElementById("studentForm").reset();
});

function displayStudents() {
    let table = document.getElementById("studentTable");
    table.innerHTML = "";

    if (students.length === 0) {
        table.innerHTML = `
            <tr>
                <td colspan="4">No students added yet</td>
            </tr>
        `;
        return;
    }

    for (let i = 0; i < students.length; i++) {
        let row = `
            <tr>
                <td>${students[i].name}</td>
                <td>${students[i].dept}</td>
                <td>${students[i].reg}</td>
                <td>
                    <button type="button" onclick="deleteStudent(${i})">
                        Delete
                    </button>
                </td>
            </tr>
        `;

        table.innerHTML += row;
    }
}

function deleteStudent(index) {
    students.splice(index, 1);
    displayStudents();
}
