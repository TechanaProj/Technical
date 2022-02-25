$(document).ready(function () {
    $(".main-menu .sb-menu > li .selected-ar").click(function () {
       
        if ($(this).parent("li").find(".sb-menu1").css("display") === "none") {            
            $(".main-menu .sb-menu > li").removeClass("active");
            $(this).parent("li").addClass("active");
            $(".main-menu .sb-menu > li .selected-ar").children(".icn1").removeClass("active");
            $(this).children(".icn1").addClass("active");
            $(".main-menu .sb-menu > li").find(".sb-menu1").slideUp("slow");
            $(this).parent("li").find(".sb-menu1").slideDown("slow");
            $(".main-menu .sb-menu1 > li .selected-ar").children(".icn1").removeClass("active");
            $(".main-menu .sb-menu1 > li").find(".sb-menu2").slideUp("slow");
            $(".main-menu .sb-menu2 > li").find(".sb-menu3").slideUp("slow");

            $(".main-menu .sb-menu1 li").removeClass("active");
            $(".main-menu .sb-menu2 li").removeClass("active");

            $(".main-menu .sb-menu3 li").removeClass("active");
        }
        else {
            $(this).parent("li").removeClass("active");
            $(this).children(".icn1").removeClass("active");
            $(this).parent("li").find(".sb-menu1").slideUp("slow");
        }
    });
    $(".main-menu .sb-menu1 > li .selected-ar").click(function () {
        if ($(this).parent("li").find(".sb-menu2").css("display") === "none") {
            $(".main-menu .sb-menu1 > li").find(".sb-menu2").slideUp("slow");
            $(this).parent("li").find(".sb-menu2").slideDown("slow");
            $(".main-menu .sb-menu1 > li .selected-ar").children(".icn1").removeClass("active");
            $(this).children(".icn1").addClass("active");
            $(".main-menu .sb-menu1 > li").removeClass("active");
           // $(this).parent("li").addClass("active");
        }
        else {
            
            
            $(this).parent("li").find(".sb-menu2").slideUp("slow");
            $(this).children(".icn1").removeClass("active");
            $(this).parent("li").removeClass("active");
            $(this).parent("li").addClass("active");
        }
    });

    $(".main-menu .sb-menu2 > li .selected-ar").click(function (e) {
        //e.preventDefault();
        //document.getElementById("page-wrapper").innerHTML = "";        
        //$('#page-wrapper').load($(this).find("a").attr('href'));
        //$(".top-head").text($(this).text());
        if ($(this).parent("li").find(".sb-menu3").css("display") === "none") {
            
            $(".main-menu .sb-menu2 > li").find(".sb-menu3").slideUp("slow");
            $(this).parent("li").find(".sb-menu3").slideDown("slow");
            $(".main-menu .sb-menu2 > li .selected-ar").children(".icn1").removeClass("active");
            $(this).children(".icn1").addClass("active");
            $(".main-menu .sb-menu2 > li").removeClass("active");
            $(this).parent("li").addClass("active");
        }
        else {
            
            $(this).parent("li").find(".sb-menu3").slideUp("slow");
            $(this).children(".icn1").removeClass("active");
            $(this).parent("li").removeClass("active");
        }
    });
    $(".main-menu .sb-menu2 li").click(function () {        
        $(".main-menu .sb-menu2 li").removeClass("active");
        $("body").toggleClass("active");
        $(this).addClass("active");
    });

    $(".main-menu .sb-menu3 li").click(function () {
        $(".main-menu .sb-menu3 li").removeClass("active");
        $(this).addClass("active");
    });

    $(".main-menu .mn-menu li").click(function () {
        $(".main-menu .mn-menu li").removeClass("active");
        $(this).addClass("active");
        // GetMenu($(this).attr("getmenu"));
        var selectLi = $(this).index();
        $(".sb-menu").hide();
        $(".sb-menu").eq(selectLi).slideDown("slow");
        $(".main-menu .sb-menu li").removeClass("active");
        $(".main-menu .sb-menu1 li").removeClass("active");
        $(".main-menu .sb-menu2 li").removeClass("active");
        $(".main-menu .sb-menu > li").find(".sb-menu1").slideUp("slow");
        $(".main-menu .sb-menu1 > li").find(".sb-menu2").slideUp("slow");
        $(".main-menu .sb-menu2 > li").find(".sb-menu3").slideUp("slow");


    });

    //$(".header .hdr-right").hover(function () {
    //    $(".header-box1").toggle();
    //   //event.stopPropagation();

    //});

 

});

