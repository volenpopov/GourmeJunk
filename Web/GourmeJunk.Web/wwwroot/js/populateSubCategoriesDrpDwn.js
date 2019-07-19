let categoryDropdown = document.getElementById("CategoryId");
debugger;
categoryDropdown.addEventListener("change", updateSubCategoryDrpDwn);

function updateSubCategoryDrpDwn() {    
    let selectedCategory = categoryDropdown.value;

    let subCategoryDrpDwn = document.getElementById("SubCategoryId");

    //Clear subcategory list
    while (subCategoryDrpDwn.firstChild) {
        subCategoryDrpDwn.removeChild(subCategoryDrpDwn.firstChild);
    }

    let url = "/Admin/SubCategory/GetSubCategories/" + selectedCategory;

    let defaultOption = document.createElement("option");
    defaultOption.setAttribute("selected", true);  
    defaultOption.value = -1;
    defaultOption.text = "Select if applicable...";

    subCategoryDrpDwn.add(defaultOption);

    fetch(url)
        .then(response => response.json())
        .catch(error => console.error('Error:', error))
        .then(data => {                       
            Array.from(data).forEach(element => {
                let option = document.createElement("option");
                option.value = element.value;
                option.textContent = element.text;
                
                subCategoryDrpDwn.add(option);
            })
        })
}