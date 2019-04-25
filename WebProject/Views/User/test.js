var gender = $("#gender").val();
if (gender == "Boy") {
	$(".genderCheck")[0].checked = true



}
else if (gender == "Girl") {
	$(".genderCheck")[1].checked = true
}
else {
	$(".genderCheck")[2].checked = true;
}
