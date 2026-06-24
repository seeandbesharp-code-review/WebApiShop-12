
// Client-side retry mechanism: retries transient failures (network errors,
// 429 rate limiting and 5xx) with exponential backoff.
function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function fetchWithRetry(url, options = {}, retries = 3, backoff = 500) {
    for (let attempt = 0; ; attempt++) {
        try {
            const response = await fetch(url, options);
            const isTransient = response.status === 429 || response.status >= 500;
            if (isTransient && attempt < retries) {
                const retryAfter = parseInt(response.headers.get("Retry-After"), 10);
                const wait = Number.isNaN(retryAfter) ? backoff * Math.pow(2, attempt) : retryAfter * 1000;
                await delay(wait);
                continue;
            }
            return response;
        } catch (err) {
            if (attempt < retries) {
                await delay(backoff * Math.pow(2, attempt));
                continue;
            }
            throw err;
        }
    }
}

async function getInfo() {
    const response = await fetchWithRetry("api/Users");
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
    const response = await fetchWithRetry("api/Users", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    if (!response.ok) {
        error = await response.text();
        alert(error);
        return;
    }
    alert("You have successfully registered.")
}

async function login() {

    if (response.status == 204) {
        alert("UserName or password is wrong, please try again.")
        return;
    }
    if (!response.ok) {
        alert("Somesing went wrong.");
        return;
    }

    UserName = document.querySelector('#lusername').value;
    Password = document.querySelector('#lpassword').value;
    const data = {
        UserName,
        Password
    };  

    const response = await fetchWithRetry("api/Users/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
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
    const response = await fetchWithRetry("api/Passwords", {
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