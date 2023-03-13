$(document).ready(function () {
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
