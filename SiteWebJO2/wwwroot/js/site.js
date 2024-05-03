
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
    
    document.getElementById("MsgModalTitle").textContent = title;
    document.getElementById("MsgModalText").textContent = text;
    const myModal = document.getElementById("MsgModal");
    const myModalElement = new bootstrap.Modal(myModal);
    return myModalElement;
}


// display tickets selected in the shopping cart page
// using data in cookie "jo2024Cart"
function displayShoppingCart() {
    //get the shoppingCart cookie string (array of ticket)
    let shoppingCartCookie = getCookie("jo2024Cart");
    console.log(shoppingCartCookie);

    // if nothing in shopping cart, display "nothing in cart still"
    if (shoppingCartCookie.length == 0) {
        document.getElementById("shoppingCartTable").hidden = true;
        document.getElementById("subtotal").hidden = true;
        document.getElementById("ContinueToPayment").hidden = true;
        var msg = document.createElement("span");
        msg.textContent = "Nothing in cart still";
        document.getElementById("shoppingCartTablePosition").appendChild(msg);
        return;
    }
    else {

        // display tickets 
        let lastTicket = {
            'joTicketPackId': -1,
            'joSessionId': -1
        };
        let countLine = 0;
        let subtotal = 0;
        let promiseArray = [];
        shoppingCartCookie.forEach((t) => {
            if (JSON.stringify(t) != JSON.stringify(lastTicket)) {

                // ticket not already exists => add a new line

                // get data of joSession and joTicketPack
                promiseArray.push(
                    PostToController("/ShoppingCarts/GetTicketData", t)
                        .then((donnees) => {
                            //create line in shopping cart
                            let nb = countSameTicket(t, shoppingCartCookie);
                            subtotal = subtotal + createTicketCard(countLine, donnees, nb);
                            ++countLine;
                            console.log(countLine);
                        })
                    );
            }
            lastTicket = t;
        });

        // display subtotal when all promises done
        Promise.all(promiseArray)
            .then(() => {
                let container = document.getElementById("subtotal");
                container.innerHTML = "Subtotal <small>(VAT incl.)</small> " + subtotal.toFixed(2) + "€";
            })
            .catch(err => console.log('error, not all promises createTicketCard finished', err)); 

    }
}


async function PostToController(url, data) {

    try {
        const reponse = await fetch(url, {
            method: "POST",
            body: JSON.stringify(data),
            headers: {
                "Content-Type": "application/json"
            }
        });

        const resultat = await reponse.json();
        console.log("Fetch request done :", resultat);
        return resultat;
    } catch (erreur) {
        console.error("Error on fetch request :", erreur);
        return null;
    } 
}



// function to create a new line for a ticket in shopping cart
function createTicketCard(countCard, ticket, nb) {
    let container = document.getElementById("shoppingCartTableBody");

    // create HTML card
    // create row
    let rowEl = document.createElement("tr");
    rowEl.id = "card" + countCard;
    container.appendChild(rowEl);
    // create row header
    let headerEl = document.createElement("th");
    headerEl.scope = "row";
    headerEl.className = "d-none";
    headerEl.textContent = JSON.stringify(ticket);
    rowEl.appendChild(headerEl);

    // create table img
    let dataImgEl = document.createElement("td");
    rowEl.appendChild(dataImgEl);
    let ImgEl = document.createElement("img");
    ImgEl.src = "" + ticket.JoSessionImage.replace("~", "") + "";
    ImgEl.alt = ticket.JoSessionImage;
    ImgEl.height = "40";
    ImgEl.className = "logoSport";
    dataImgEl.appendChild(ImgEl);

    // create table data description session
    let dataDescDiv = document.createElement("td");
    rowEl.appendChild(dataDescDiv);
    let SessionNameEl = document.createElement("h5");
    SessionNameEl.textContent = ticket.JoSessionName;
    SessionNameEl.style = "margin:0";
    dataDescDiv.appendChild(SessionNameEl);
    let SessionDetailsEl = document.createElement("p");
    SessionDetailsEl.innerHTML = ticket.JoSessionPlace + "<br>" + FormatDate(ticket.JoSessionDate) + "<br>" + ticket.JoTicketPackName + " (" + ticket.NbAttendees + " attendees)";
    SessionDetailsEl.style = "margin:0";
    dataDescDiv.appendChild(SessionDetailsEl);

    // create dropdown button for number of tickets
    let dataButtonEl = document.createElement("td");
    dataButtonEl.style = "text-align:center" ;
    dataButtonEl.className = "dropdown";
    rowEl.appendChild(dataButtonEl);
    let BtnEl = document.createElement("button");
    BtnEl.style = "background-color:white";
    BtnEl.className = "btn dropdown-toggle btn-outline-secondary NbTicketButton";
    BtnEl.type = "button";
    BtnEl.id = "NbTicketButton" + countCard;
    //BtnEl.setAttribute("id", "NbTicketButton" + countCard);
    BtnEl.setAttribute("data-bs-toggle", "dropdown");
    BtnEl.ariaExpanded = "false";
    BtnEl.textContent = nb;
    dataButtonEl.appendChild(BtnEl);
    let BtnList = document.createElement("ul");
    BtnList.className = "dropdown-menu";
    BtnList.setAttribute('aria-labelledby', "NbTicketButton" + countCard);
    dataButtonEl.appendChild(BtnList);
    for (let i = 0; i < 4; i++) {
        let BtnListEl = document.createElement("li");
        let BtnListLink = document.createElement("a");
        BtnListLink.className = "dropdown-item";
        BtnListLink.href = "#";
        BtnListLink.textContent = i;
        BtnListEl.appendChild(BtnListLink);
        BtnList.appendChild(BtnListEl);
    }

    // create table data "price each"
    let dataPriceOneEl = document.createElement("td");
    dataPriceOneEl.style = "text-align:center";
    rowEl.appendChild(dataPriceOneEl);
    let PriceOneEl = document.createElement("h5");
    PriceOneEl.textContent = ticket.JoPackPrice.toFixed(2) + "€";
    PriceOneEl.style = "margin:0";
    dataPriceOneEl.appendChild(PriceOneEl);

    // create table data "total price"
    let dataPriceTotalEl = document.createElement("td");
    dataPriceTotalEl.style = "text-align:center";
    rowEl.appendChild(dataPriceTotalEl);
    let PriceTotalEl = document.createElement("h5");
    let priceTotal = ticket.JoPackPrice * nb;
    PriceTotalEl.textContent = priceTotal.toFixed(2) + "€";
    PriceTotalEl.style = "margin:0";
    dataPriceTotalEl.appendChild(PriceTotalEl);

    return priceTotal;
}

// function to format the date to display it in html code
function FormatDate(myDate) {
    const utcDate = new Date(myDate);

    return utcDate.toUTCString();
}

// function to count the number of ticket 'ticket' is in 'cart'
function countSameTicket(ticket, cart) {
    countSameticket = 0;
    cart.forEach((t) => {
        if (JSON.stringify(t) === JSON.stringify(ticket)) {
            ++countSameticket;
        }
    });
    return countSameticket;
}