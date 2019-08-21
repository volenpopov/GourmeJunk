let statusMessage = document.getElementById("statusMessage").value;

(function () {
    let couponCode = document.getElementById("couponCode").value;

    console.log(statusMessage);

    if (couponCode.length > 0 && !statusMessage) {
        document.getElementById('btnApplyCoupon').style.display = 'none';
        document.getElementById('btnRemoveCoupon').style.display = '';
    }
    else {
        document.getElementById('btnApplyCoupon').style.display = '';
        document.getElementById('btnRemoveCoupon').style.display = 'none';
    }
})()