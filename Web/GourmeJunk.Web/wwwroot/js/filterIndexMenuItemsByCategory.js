var posts = document.getElementsByClassName("post");

var categoryList = Array.from(document.querySelectorAll("#menu-filters li"));

categoryList.forEach(category =>
    category.addEventListener("click", displaySelectedList));

function displaySelectedList() {
    let selectedCategory = this;

    categoryList.forEach(function (category) {
        category.classList.remove("active", "btn", "btn-secondary");
    });

    selectedCategory.classList.add("active", "btn", "btn-secondary");
    selectedCategory.setAttribute("data-filter", "filter");

    console.log(selectedCategory.textContent);

    Array.from(document.getElementsByClassName("menu-restaurant"))
        .forEach(function (menuItemsList) {
            let selectedCategoryName = selectedCategory.textContent;

            if (menuItemsList.classList.contains(selectedCategoryName)) {
                menuItemsList.style.display = "block";
                return;
            } else if (selectedCategoryName === "Show All") {
                menuItemsList.style.display = "block";                
                return;
            }

            menuItemsList.style.display = "none";
        });
}