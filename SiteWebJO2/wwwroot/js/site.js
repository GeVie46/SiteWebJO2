// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// add a ticket to ShoppingCart
// store each ticket as an object in an array in json format, and save in a cookie
function addTicket(joTicketPackId, joSessionId) {
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
    alert("the ticket is in your shopping cart " + JSON.stringify(shoppingCartCookie));
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
