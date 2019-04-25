$("#pagination li").click(function () {
	var activeIndex = parseInt($("#pageCount").val());
	var index = $(this).index();
	if (index == activeIndex) {
		return;
	}


	var length = $(this).parent().children().length;
	if (index == 0)//previous
	{

		if (activeIndex == 1)//第一页
		{
			return;
		}
		$(this).parent().children().eq(activeIndex).removeClass('active');
		var nextIndex = activeIndex - 1;
		$(this).parent().children().eq(nextIndex).addClass('active');
		$("#pageCount").val(nextIndex);
		GetPictures(nextIndex);
		return;

	}
	if (index == length - 1) {
		if (activeIndex == length - 2) {
			return;
		}
		$(this).parent().children().eq(activeIndex).removeClass('active');
		var nextIndex = activeIndex + 1;
		$(this).parent().children().eq(nextIndex).addClass('active');
		$("#pageCount").val(nextIndex);
		GetPictures(nextIndex);
		return;
	}
	$(this).parent().children().eq(activeIndex).removeClass('active');
	$(this).parent().children().eq(index).addClass('active');
	var nextIndex = index;
	$("#pageCount").val(nextIndex);
	GetPictures(nextIndex);
	return;


}
)