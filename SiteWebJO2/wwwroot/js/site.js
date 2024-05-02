// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// add a ticket to ShoppingCart
// store each ticket as an object in an array in json format, and save in a cookie
function addTicket(joTicketPackId, joSessionId) {

    //check if user is logged in. If not, cancel add to cart and redirect to login page
    var signed = $('#logged').val();
    if (signed === 'false') {

        var myModalElement = customModal("Action impossible", "You must login to add tickets to your shopping cart");
        myModalElement.show();
        $(document).on('hidden.bs.modal', '#MsgModal', function () {
                window.location.href = '/Identity/Account/Login';
        });
        return;
    }

    console.log(`addTicket to Cart joTicketPackId: ${joTicketPackId}, joSessionId: ${joSessionId}`);
    //create object to store
    var ticket = {
        'joTicketPackId': joTicketPackId,
        'joSessionId': joSessionId
    };

    //get the shoppingCart cookie string (array of ticket)
    let shoppingCartCookie = getCookie("jo2024Cart");

    //add the new ticket
    shoppingCartCookie.push(ticket);

    //cookie expires in 1 hour, so if the user doesnt pass order too quick he must select again the session and we can check if tickets are still available
    let DateExp = new Date();
    DateExp.setHours(DateExp.getHours() + 1);

    //overwrite the cookie
    document.cookie = `jo2024Cart=${JSON.stringify(shoppingCartCookie)}; expires=${DateExp.toUTCString()}; path=/`;

    //inform user that ticket is added
    var myModalElement = customModal("Action done", "The ticket was added to your shopping cart");
    myModalElement.show();
}

//returns the value of a specified cookie
//param : cname : the name of the cookie searched
//code adapted from https://www.w3schools.com/js/js_cookies.asp
function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return JSON.parse(c.substring(name.length, c.length));
        }
    }
    //if cookie doesnt exist, or is null, return an empty array
    return [];
}

// custom display of bootstrap modal
// modal template defined in Layout.cshtml
function customModal(title, text) {
    
    document.getElementById("MsgModalTitle").innerHTML = title;
    document.getElementById("MsgModalText").innerHTML = text;
    const myModal = document.getElementById("MsgModal");
    const myModalElement = new bootstrap.Modal(myModal);
    return myModalElement;
}

function displayShoppingCart() {
    //get the shoppingCart cookie string (array of ticket)
    let shoppingCartCookie = getCookie("jo2024Cart");
    console.log(shoppingCartCookie);

    if (shoppingCartCookie.length != 0) {
        shoppingCartCookie.forEach((e) => {
            var postPromise = PostToController("/ShoppingCarts/GetTicketData", e);
            alert(postPromise);
        });
    }
}


function PostToController(url, data) {

    fetch(url, {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
            "Content-Type": "application/json"
        }
    })
    .then(response => {
        if (response.ok) {
            console.log('Json Response:', json);
            return response.json();
        }
        else {
            console.log('Json Response:', json)
            console.log('error', response);
        }
    })
    .catch(error => console.log('Request failed:', error)); 
}



