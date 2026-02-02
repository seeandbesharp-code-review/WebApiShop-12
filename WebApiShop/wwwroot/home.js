
async function getInfo() {
    const response = await fetch("api/Users");
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    const data = await response.json();
    alert(data);
}


async function register() {
    UserName = document.querySelector('#userName').value;
    Password = document.querySelector('#password').value;
    FirstName = document.querySelector('#firstName').value;
    LastName = document.querySelector('#lastName').value;
    const data = {
        UserName,
        Password,
        FirstName,
        LastName 
    };  
    const response = await fetch("api/Users", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    if (!response.ok) {
        alert("Something went wrong, please try again.");
        return;
    }
    alert("You have successfully registered.")
}

async function login() {
    UserName = document.querySelector('#lusername').value;
    Password = document.querySelector('#lpassword').value;
    const data = {
        UserName,
        Password
    };  

    const response = await fetch("api/Users/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });

    if (response.status == 204) {
        alert("UserName or password is wrong, please try again.")
        return;
    }
    if (!response.ok) {
        alert("Somesing went wrong.");
        return;
    }

    const user = await response.json();
    sessionStorage.setItem('user', JSON.stringify(user));
    window.location.href = "src\\update.html";
}

function reg() {
    reg = document.querySelector(".reg");
    reg.style.display = "block";

}


async function checkPassword() {
    data = document.querySelector('#password').value;
    const response = await fetch("api/Passwords", {
        method : 'POST',
        headers: {
            "Content-Type": 'application/json'
        },
        body: JSON.stringify(data)
    })
    level = await response.json();
    document.querySelector("progress").style.display = "block";
    document.querySelector("progress").value = level;
}