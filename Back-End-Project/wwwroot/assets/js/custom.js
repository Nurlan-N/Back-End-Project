$(document).ready(function () {
    
    $('.addToBasket').click(function myFunction(e) {
        e.preventDefault();

        let productId = $(this).data('id');
        fetch('basket/AddBasket?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.header-cart').html(data)
                 
                $(".offcanvas-close, .minicart-close,.offcanvas-overlay").on('click', function () {
                    $("body").removeClass('fix');
                    $(".offcanvas-search-inner, .minicart-inner").removeClass('show')
                })
            })
       
    })
    $(document).on('click', '.deleteToBasket' , function(e) {
        e.preventDefault();

        const removeId = $(this).attr('data-id');

        fetch('basket/DeleteBasket?id=' + removeId)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.header-cart').html(data)
                $(".offcanvas-search-inner, .minicart-inner").addClass('show')
                $(".offcanvas-close, .minicart-close,.offcanvas-overlay").on('click', function () {
                    $("body").removeClass('fix');
                   $(".offcanvas-search-inner, .minicart-inner").removeClass('show')
                })
            })
       
    })
    $('#SearchValue').keyup(function ()
	{
        let search = $(this).val();
        console.log(search)
        if (search.Trim().length >= 3) {
            fetch('product/search?search=' + search)
                .then(res => {
                    return res.text()
                }).then(data => {
                    $('.searchBody').html(data)
                })
        } else {
            $('.searchBody').html('')

        }
	})


    $(".productModal").click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href')

        fetch(url).then(res =>
        {
            return res.text();
        })
            .then(data => {
                $('.modal-content').html(data)
				// prodct details slider active
				$('.product-large-slider').slick({
					fade: true,
					arrows: false,
					asNavFor: '.pro-nav'
				});


				// product details slider nav active
				$('.pro-nav').slick({
					slidesToShow: 4,
					asNavFor: '.product-large-slider',
					arrows: false,
					focusOnSelect: true
				});

            })
    })
})
