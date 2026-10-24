
const contactForm = document.getElementById("contactForm");
const formMessage = document.getElementById("formMessage");
const yearElement = document.getElementById("year");

yearElement.textContent = new Date().getFullYear();

contactForm.addEventListener("submit", function (event) {
    event.preventDefault();

    formMessage.textContent =
        "Thank you! Your demo enquiry has been recorded on this page.";

    formMessage.className = "mt-3 text-success fw-semibold";

    contactForm.reset();
});
