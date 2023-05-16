
document.getElementById("btn").addEventListener("click", function () {
    console.log("Alert");
    Swal.fire({
        title: 'Error!',
        text: 'Do you want to continue',
        icon: 'error',
        confirmButtonText: 'Cool'
    })
})
