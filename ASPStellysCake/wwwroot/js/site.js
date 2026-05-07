
//    const checkboxes = document.querySelectorAll(".gallery-filters input");
//    const cards = document.querySelectorAll(".gallery-card");

//checkboxes.forEach(cb => {
//        cb.addEventListener("change", filterImages);
//});

//    function filterImages() {
//    const selected = Array.from(checkboxes)
//        .filter(c => c.checked)
//        .map(c => c.value);

//    cards.forEach(card => {
//        const category = card.dataset.category;

//    if (selected.length === 0 || selected.includes(category)) {
//        card.style.display = "block";
//        } else {
//        card.style.display = "none";
//        }
//    });
//}

//document.querySelector("form").addEventListener("submit", function(e) {
//    e.preventDefault();

//    document.getElementById("successMessage").style.display = "block";
//});
