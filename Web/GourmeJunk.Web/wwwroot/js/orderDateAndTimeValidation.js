        $(function () {
            $("#datepicker").datepicker({ minDate: new Date(), maxDate: "+3", dateFormat: "dd/mm/yy" });

            $('#timepicker').timepicker({
                timeFormat: 'HH:mm',
                interval: 30,
                minTime: '11',
                maxTime: '22',
                startTime: '11',
                dropdown: true,
                scrollbar: true
            });
        });      

        function validateInput() {
            let time = document.getElementById("timepicker").value;
            let date = document.getElementById("datepicker").value;
            let name = document.getElementById("txtName").value;
            let phoneNumber = document.getElementById("phone").value;

            let today = new Date();
            let currentHours = today.getHours();
            let currentDate = today.getDate();

            let selectedHours = Number(time.substring(0, 2));
            let selectedDate = Number(date.substring(0,2));

            if (time.toString() === '') {
                alert("Please select pickup time!");
                return false;
            } else if (date.toString() === '') {
                alert("Please select pickup date!");
                return false;
            } else if (name.toString() === '') {
                alert("Please enter pickup name!");
                return false;
            } else if (phoneNumber.toString() === '' || !phoneNumber.match(/^[0-9]{7,}$/)) {
                alert("Please enter a valid phone number!");
                return false;
            }            

            if (selectedDate <= currentDate) {
                if (selectedDate < currentDate) {
                    alert("Please select a valid date!");
                    return false;
                } else if (selectedDate === currentDate) {
                    if (selectedHours < currentHours + 1) {
                        alert("We need more time to prepare your order. Please select a later hour!");
                        return false;
                    }
                }                
            }            
        }