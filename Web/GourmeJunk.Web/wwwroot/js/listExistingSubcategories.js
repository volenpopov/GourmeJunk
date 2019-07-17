let categoryDropdown = document.getElementById("ddlCategorylist");

function updateSubCategoryDrpDwn() {
    let selectedCategory = categoryDropdown.value;

    let subCategoryListDiv = document.getElementById("SubCategoryList");

    //Clear subcategory list
    while (subCategoryListDiv.firstChild) {
        subCategoryListDiv.removeChild(subCategoryListDiv.firstChild);
    }

    let url = "/Admin/SubCategory/GetSubCategories/" + selectedCategory;

    fetch(url)
        .then(response => response.json())
        .catch(error => console.error('Error:', error))
        .then(data => {
            let list = document.createElement("ul");
            list.classList.add("list-group");
            list.classList.add("list-group-flush");

            subCategoryListDiv.appendChild(list);

            Array.from(data).forEach(element => {
                let item = document.createElement("li");
                item.classList.add("list-group-item");
                
                item.textContent = element.text;

                list.appendChild(item);
            })
        })
}

categoryDropdown.addEventListener("change", updateSubCategoryDrpDwn);