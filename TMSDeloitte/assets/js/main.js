$(document).ready(function(){
	$("#menu-collapse").click(function(){
		
		if($("body").hasClass("modern-layout")) {
			$("body").removeClass("modern-layout").addClass("minimenu")
		} else {
			$("body").removeClass("minimenu").addClass("modern-layout")
		}
	});
});