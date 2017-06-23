if (!Modernizr) {
    console.warn('Modernizr is required');
};
if (!$()) {
    console.warn('JQuery is required');
};

(function($) {
    'use strict';
    var CCS = {
        init: function() {
            CCS.paceloader();
            CCS.scrollChecker();
            CCS.searchFocus();
            CCS.slickSlider();
            CCS.navTriggerMobile();
            CCS.navigation();
            CCS.mobileSearch();
            CCS.mobileLinkDropdown();
            //CCS.counterUps();
            CCS.sideNavigation();
            CCS.socialShare();
            CCS.sidenavToggle();
            CCS.tabScroll();
            CCS.itemSlider();
            CCS.pdtSlider();
        },

        settings: {
            tabScreen: 1024,
            mobScreen: 768,
        },

        // Pace loader
        paceloader: function() {
            Pace.on('done', function() {
                $('body').addClass('loaded');
                CCS.setClass();

            });
        },
        //side navigation accordion
        sideNavigation: function() {
            var linkHandler = $('.side-nav span.arrow');
           //$('.inner-menu').each(function() {
                //if ($(this).find('.active').length) {
                    //$(this).slideDown();
                    //$($(this).closest('.active').find('.arrow')).addClass('open');
                //} else {
                    //$(this).slideUp();
                //}
            //});
			 $('.side-nav-ul li').each(function(){
                if(!$($(this).find('.arrow')).hasClass('open')){
                    $(this).find('.inner-menu').hide();
                }
            })
            

            linkHandler.on('click', function() {
                if ($(this).hasClass('open')) {
                    linkHandler.removeClass('open');
                    $(this).next('.inner-menu').slideUp();
                } else {
                    linkHandler.removeClass('open');
                    $('.inner-menu').slideUp();
                    $(this).addClass('open');
                    $(this).next('.inner-menu').slideDown();
                }
            });
        },
        // Loader 
        mainLoader: function() {
            var loader = $('#main-loader');
            var wrapper = $('#main-wrap');

            var tl = new TimelineLite();


            tl.to(loader, 1, { left: "50%" })
                .to(loader, .5, { opacity: 0, display: "none" }, 'loader');

            // Appending slider navigation after mainloader is done

            tl.add(CCS.bannerNavEntrance);
        },

        bannerNavEntrance: function() {
            var tl = new TimelineLite();
            var banner_slider_nav_prev = $('.banner-slider .slick-prev');
            var banner_slider_nav_next = $('.banner-slider .slick-next');

            tl.to(banner_slider_nav_prev, .5, { opacity: 1, x: 0 }, 'prev-nav')
                .to(banner_slider_nav_next, .5, { opacity: 1, x: 0 }, 'prev-nav');

            return tl;

        },

        bannerAnimations: {
            bannerElements: {
                bannerContainer: $('.banner-wrapper'),
                bannerImage: $('.banner-slider .bg-figure'),
                bannerContent: $('.banner-slider .banner-content'),
            },
            parallax: function() {
                var scrollController = CCS.scrollMagicController()

                var tween = TweenLite.fromTo(this.bannerElements.bannerImage, .5, { y: '0%', rotation: .01 }, { y: '100%', rotation: .01, ease: Linear.easeNone }, .5);
                var scene = new ScrollMagic.Scene({ duration: 2000 })
                    .setTween(tween)
                    .addTo(scrollController);
            },
            bannerContentVisibility: function() {
                var scrollController = CCS.scrollMagicController()
            }

        },

        // ScrollMagic Controller
        scrollMagicController: function(options) {
            var controller = new ScrollMagic.Controller(options);
            return controller;
        },

        // SetClass on view uses scrollMagic
        setClass: function(option) {
            var slides = document.querySelectorAll(".magic");
            var controller = CCS.scrollMagicController();
            for (var i = 0; i < slides.length; i++) {
                var attribute_triggerElement = slides[i].getAttribute("data-trigger");
                var attribute_offset = slides[i].getAttribute("data-offset");
                var attribute_hook = slides[i].getAttribute("data-hook");
                var attribute_reverse = slides[i].getAttribute("data-reverse");
                var attribute_animation_delay = slides[i].getAttribute("data-animation-delay");
                var triggerElement = attribute_triggerElement ? attribute_triggerElement : slides[i];
                var offset = attribute_offset ? attribute_offset : 100;
                var hook = attribute_hook ? attribute_hook : 1;
                var reverse = attribute_reverse ? true : false;
                var animation_delay = attribute_animation_delay ? attribute_animation_delay : 0;

                $(slides[i]).css({ 'animation-delay': animation_delay });

                new ScrollMagic.Scene({
                        triggerElement: triggerElement,
                        offset: parseInt(offset),
                        triggerHook: parseInt(hook),
                        reverse: reverse
                    })
                    .setClassToggle(slides[i], 'animated')
                    .addTo(controller);
            }
        },
        socialShare: function() {
            $('.social-share-block').hover(function() {
				$('.share-links').toggleClass('open');
                $(this).toggleClass('open');
            });
        },
        sidenavToggle: function() {
            $('.btn-accordion').on('click', function() {
                $('.side-nav-ul').toggleClass('open');
            });
        },
        searchFocus: function() {
            var formHolder = $('.search-form'),
                button = formHolder.find('.button'),
                textFeild = formHolder.find('.form-control');
            $(window).on('load resize', function() {
                if ($(window).width() > CCS.settings.mobScreen) {
                    textFeild.on('focus blur', function() {
                        $(this).parents('.search-form').toggleClass('open');
                    });
                } else {
                    button.on('click', function(e) {
                        e.preventDefault();
                        $(this).parents('.search-form').toggleClass('open');

                    })
                }
            });
        },
        mobileSearch: function() {
            var formHolder = $('.mob-search-form'),
                openButton = $('.mob-search-form .open-search-form'),
                closeButton = $('.mob-search-form .close-search-form');
            openButton.click(function(e) {
                e.preventDefault();
                $(this).parents('.mob-search-form').addClass('open');
            });
            closeButton.click(function(e) {
                e.preventDefault();
                $(this).parents('.mob-search-form').removeClass('open');
            });

        },
        popups: function() {
            var triggerElement = $('.trigger-popup');
            triggerElement.click(function() {
                var target = $(this).attr('href');
                var elemBottom = $(this).offset().top + $(this).outerHeight();
                console.log($(this).height());
                var elemLeft = $(this).offset().left - $(this).outerWidth();

                $(target).css({ 'top': elemBottom, 'left': elemLeft }).fadeIn();
            })
        },

        navTriggerMobile: function() {
            var trigger = $('#toggle-menu');
            var target = $(trigger.data('target'));
            trigger.show();
            //$('#main-nav-wrapper').outerHeight($(window).height()); 

            trigger.click(function() {
                if (!$(this).hasClass('open')) {
                    $(this).addClass('open');
                    TweenLite.to(target, .5, { x: '0%', opacity: 1 });

                    //$(target).scrollTop($(target).offset().top - 100);
                    $("body").addClass('menu-open');
                } else {
                    $(this).removeClass('open');
                    TweenLite.to(target, .5, { x: '-100%', opacity: 0 });
                    $("body").removeClass('menu-open');
                }
            });


            // Nav Height

            $(window).on('load resize', function() {

                $('#main-nav-wrapper').outerHeight($(window).outerHeight() - $('.main-header').outerHeight());

            })


        },
        navigation: function() {
            var link = $('.submenu>a');
            link.on('click', function(e) {
                if (!$(this).parent().hasClass('hover')) {
                    e.preventDefault();
                    if (!$(this).parent().hasClass('open')) {
                        $('.nav-wrapper .open').removeClass('open');
                        $('.nav-wrapper .active').removeClass('active');
                        $(this).parent().addClass('open');
                        $(this).addClass('active');
                    } else {
                        $(this).parent().removeClass('open');
                        $(this).removeClass('active');
                    }
                }

            });
        },

        scrollChecker: function() {
            $(window).on('load scroll', function() {
                if ($(window).scrollTop() > 130) {
                    $('body').addClass('scrolled');
                } else {
                    $('body').removeClass('scrolled');
                }
            });
        },

        slickSlider: function() {
            var slider = $('.slider-major');
            var speedSet = parseInt(slider.data('speed'));
            var autoplaySpeedSet = parseInt(slider.data('autospeed'));
            var arrow_left = $('.arrow-left');
            var arrow_right = $('.arrow-right');
            if (slider.find('>div').length == 1) {

                $('.slider-controls').hide();
            }

            if (slider.find('>div').length > 1) {

                slider.slick({
                    fade: true,
                    dots: true,
                    arrows: false,
                     autoplay: true,
                    speed: speedSet,
                    autoplaySpeed: autoplaySpeedSet,
                    // adaptiveHeight: true,
                    appendDots: $("#pagination-wrap")
                });
            }

            arrow_left.click(function() {
                slider.slick('slickPrev');
            });
            arrow_right.click(function() {
                slider.slick('slickNext');
            });
        },
        pdtSlider: function() {
            var slider = $('.pdt-slider');
            var arrow_left = $('.arrow-left');
            var arrow_right = $('.arrow-right');
            slider.slick({
                dots: true,
                arrows: false,
                autoplay: true,
                //adaptiveHeight: true,
                appendDots: $("#pagination-wrap")
            });
            arrow_left.click(function() {
                slider.slick('slickPrev');
            });
            arrow_right.click(function() {
                slider.slick('slickNext');
            });
        },

        // Mobile link click

        mobileLinkDropdown: function() {
            $(window).on('load resize', function() {
                if ($(window).width() < CCS.settings.tabScreen) {
                    $('.main-nav .submenu .caret').click(function() {
                        if (!$(this).parent().hasClass('open')) {
                            $('.nav-wrapper .open').removeClass('open');
                            $(this).parent().addClass('open');
                        } else {
                            $(this).parent().removeClass('open');
                        }
                    });
                }
            });
        },

        mainNavWrapperHeight: function() {

            $(window).on('load resize', function() {
                if ($(window).width() < CCS.settings.tabScreen) {

                    $('#main-nav-wrapper').height($(window).outerHeight() - $('.main-header').outerHeight());
                }

            });

        },

        // counterUps: function() {
        //     $('.counter').counterUp({
        //         delay: 10,
        //         time: 1000
        //     });
        // },

        tabScroll: function() {
            var tabNavs = $('.tabs-cont-wrap .nav-tabs');
            var tabContents = $('.tabs-cont-wrap .tab-content');
			
			$('.tab-slider').slick({
                slidesToScroll: 1,
                infinite: false,
                dots: false,
                arrows: false,
                focusOnSelect: true,
                variableWidth: true,
                draggable: true,
            });
            tabNavs.slick({
                slidesToScroll: 1,
                slide: '.tabs-cont-wrap .nav-tabs li',
                infinite: false,
                dots: false,
                arrows: false,
                asNavFor: tabContents,
                focusOnSelect: true,
                variableWidth: true,
                draggable: true,
            });
            tabContents.slick({
                slidesToShow: 1,
                slide: '.tabs-cont-wrap .tab-content .tab-pane',
                dots: false,
                arrows: false,
                asNavFor: tabNavs,
                fade: true,
                infinite: false,
                draggable: false,
                adaptiveHeight: true
            })

        },
        itemSlider: function() {
            $('.item-slider').slick({
                infinite: true,
                slidesToShow: 4,
                slidesToScroll: 1,
                autoplay: true,
                speed: 300,
                responsive: [{
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3,
                        infinite: true,
                        dots: true
                    }
                }, {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2
                    }
                }, {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }]
            });
        }
    };

    CCS.init();
}(jQuery));


// // Select option list
if ($('.selectpicker').selectpicker) {
    // Native on smaller devices
    if ($(window).innerWidth() < 768 || /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
        $('.selectpicker').selectpicker('mobile');
    }
}



// Switch Banner images between mobile and desktop
ImageSwap = function(query) {
    this.$div = $(query);
    this._mobileWidth = 767;
    this._isMobile = true;
    this._init();
}
ImageSwap.prototype = {
    _init: function() {
        window.addEventListener('resize', $.proxy(this._onResize, this), false);
        this._onResize();
        this._switch();
    },
    _switch: function() {
        var src, srcData, $div_item, that = this;
        this.$div.each(function(index) {
            $div_item = $(this);
            srcData = that._isMobile ? $div_item.data('bg-mob') : $div_item.data('bg-desktop');
            // load the data src
            $div_item.css('background', 'url(' + srcData + ')');
        })
    },
    _onResize: function() {
        this._innerWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        if (this._innerWidth <= this._mobileWidth) {
            if (!this._isMobile) {
                this._isMobile = true;
                this._switch();
            }
        } else {
            if (this._isMobile) {
                this._isMobile = false;
                this._switch();
            }
        }
    }
}
var is = new ImageSwap('.img-switch');


function MobileClick(){
if ($(window).width() < 960) {

$('.box-wrapper div a').on('click mouseenter', function() {
    $(".box-wrapper div a").removeClass('active');
    $(this).addClass('active');
});
$('.box-wrapper div a').on('click mouseenter', function(e) {
        var el = $(this);
        var link = el.attr('href');
        window.location = link;
 });



$('.image-holder a').on('click mouseenter', function() {
    $(".image-holder a").removeClass('active');
    $(this).addClass('active');
});


    $('.image-holder a').on('click mouseenter', function(e) {
        var el = $(this);
        var link = el.attr('href');
        window.location = link;
    });

}
else{
	
}
}
$(document).ready(function(){
    MobileClick();
})
$(window).load(function() {
   MobileClick();
});
$(window).resize(function() {
    MobileClick();
});

equalheight = function(container) {
    var currentTallest = 0,
        currentRowStart = 0,
        rowDivs = new Array(),
        $el,
        topPosition = 0;
    $(container).each(function() {

        $el = $(this);
        $($el).height('auto')
        topPostion = $el.position().top;

        if (currentRowStart != topPostion) {
            for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
                rowDivs[currentDiv].height(currentTallest);
            }
            rowDivs.length = 0; // empty the array
            currentRowStart = topPostion;
            currentTallest = $el.height();
            rowDivs.push($el);
        } else {
            rowDivs.push($el);
            currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);
        }
        for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
            rowDivs[currentDiv].height(currentTallest);
        }
    });
}
$(document).ready(function(){
    equalheight('.eq-ht');
})
$(window).load(function() {
    equalheight('.eq-ht');
});
$(window).resize(function() {
    equalheight('.eq-ht');
});

$('.tab-dropdown-title').on('click',function(){
  $(this).next('.tab-dropdown-list').slideToggle();
})
