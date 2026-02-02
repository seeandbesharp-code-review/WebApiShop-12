
function showDetails() {
    reg = document.querySelector(".det");
    reg.style.display = "block";

    document.querySelector('#username').value = user.userName;
    document.querySelector('#password').value = user.password;
    document.querySelector('#FirstName').value = user.firstName;
    document.querySelector('#LastName').value = user.lastName;
}

if (!sessionStorage.getItem('user')) {
    alert("Please login");
    window.location.href = "../home.html";
}

title = document.querySelector("h1");
user = JSON.parse(sessionStorage.getItem('user'));
title.textContent = title.textContent + user.firstName;


async function Update() {
    UserId = user.userId;
    UserName = document.querySelector('#username').value;
    Password = document.querySelector('#password').value;
    FirstName = document.querySelector('#FirstName').value;
    LastName = document.querySelector('#LastName').value;
    const data = {
        UserId,
        UserName,
        Password,
        FirstName,
        LastName
    };  
    const response = await fetch( `../api/Users/${user.userId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        alert("Somesing went wrong.");
        return;
    }
    sessionStorage.setItem('user', JSON.stringify(data));
    alert("Details Updated successfully.")
}



