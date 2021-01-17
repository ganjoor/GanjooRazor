﻿$(function () {
    $(".search").keyup(function () {
        var searchid = $(this).val();
        var dataString = 'search=' + searchid;
        if (searchid != '') {
            $.ajax({
                type: "POST",
                url: "/spotify?handler=SearchByArtistName",
                data: dataString,
                cache: false,
                success: function (html) {
                    $("#result").html(html).show();
                }
            });
        } return false;
    });


    jQuery("#result").on("click", function (e) {
        var $clicked = $(e.target);
        var $name = $clicked.find('.name').html();
        var $artist_id = $clicked.find('.artist_id').html();
        var $artist_url = $clicked.find('.artist_url').html();
        var decoded = $("<div/>").html($name).text();

        $("#artistlink").attr('href', $artist_url);

        $('#l1').css('visibility', 'visible');

        $("#spotify_artist_name").val($name);
        $("#spotify_artist_url").val($artist_url);

        $('#searchid').val(decoded);
        if ($("#searchid").val() != '') {
            $("#searchid").attr("disabled", "disabled");
            $('#album').attr("disabled", "disabled");
            $("#track").attr("disabled", "disabled");

            $.post("/spotify?handler=FillAlbums", {
                'artist': $artist_id
            },
                function (data) {
                    var sel = $("#album");
                    sel.empty();
                    for (var i = 0; i < data.length; i++) {
                        sel.removeAttr("disabled");
                        sel.append('<option album-url="' + data[i].url + '" value="' + data[i].id + '">' + data[i].name + '</option>');
                    }

                    $("#searchid").removeAttr("disabled");




                    $('#album').trigger("change");


                }, "json");


        }
    });

    jQuery(document).on("click", function (e) {
        var $clicked = $(e.target);
        if (!$clicked.hasClass("search")) {
            jQuery("#result").fadeOut();
        }
        if (!$clicked.hasClass("trackq")) {
            jQuery("#resultq").fadeOut();
        }
    });

    $('#searchid').click(function () {
        jQuery("#result").fadeIn();

    });

    $('#trackq').click(function () {
        jQuery("#trackq").fadeIn();

    });

    $(".trackq").keyup(function () {
        var searchid = $(this).val();
        var dataString = 'search=' + searchid;
        if (searchid != '') {
            $.ajax({
                type: "POST",
                url: "/spotify?handler=SearchByTrackTitle",
                data: dataString,
                cache: false,
                success: function (html) {
                    $("#resultq").html(html).show();
                }
            });
        } return false;
    });

    jQuery("#resultq").on("click", function (e) {
        var $clicked = $(e.target);
        var $artist_name = $clicked.find('.artist_name').html();
        var $artist_id = $clicked.find('.artist_id').html();
        var $artist_url = $clicked.find('.artist_url').html();

        var $album_name = $clicked.find('.album_name').html();
        var $album_id = $clicked.find('.album_id').html();
        var $album_url = $clicked.find('.album_url').html();


        var $track_name = $clicked.find('.track_name').html();
        var $track_id = $clicked.find('.track_id').html();
        var $track_url = $clicked.find('.track_url').html();


        var decoded = $("<div/>").html($track_name).text();

        $("#artistlink").attr('href', $artist_url);
        $("#albumlink").attr('href', $album_url);
        $("#tracklink").attr('href', $track_url);

        $('#searchid').val($artist_name);

        $('#l1').css('visibility', 'visible');
        $('#l2').css('visibility', 'visible');
        $('#l3').css('visibility', 'visible');

        $("#spotify_artist_name").val($artist_name);
        $("#spotify_artist_url").val($artist_url);
        $("#spotify_album_name").val($album_name);
        $("#spotify_album_url").val($album_url);
        $("#spotify_track_name").val($track_name);
        $("#spotify_track_url").val($track_url);



        $('#trackq').val(decoded);

        var selAlbum = $("#album");
        selAlbum.empty();
        selAlbum.append('<option album-url="' + $album_url + '" value="' + $album_id + '">' + $album_name + '</option>');

        var selTrack = $("#track");
        selTrack.empty();
        selTrack.append('<option track-url="' + $track_url + '" value="' + $track_id + '">' + $track_name + '</option>');

    }

    );




    $('#album').on('change', function () {
        if ($("#album option:selected").text() != '') {
            $("#track").attr("disabled", "disabled");
            var $album_url = $("#album option:selected").attr('album-url');
            $("#albumlink").attr('href', $album_url);
            $('#l2').css('visibility', 'visible');


            $("#spotify_album_name").val($("#album option:selected").html());
            $("#spotify_album_url").val($album_url);

            $.post("/spotify?handler=FillTracks", {
                'album': $("#album option:selected").val()
            },
                function (data) {
                    var sel = $("#track");
                    sel.empty();
                    for (var i = 0; i < data.length; i++) {
                        sel.removeAttr("disabled");
                        sel.append('<option track-url="' + data[i].url + '" value="' + data[i].id + '">' + data[i].name + '</option>');
                    }
                    $("#track").removeAttr("disabled");
                    $('#track').trigger("change");



                }, "json");
        }
    });


    $('#track').on('change', function () {
        if ($("#track option:selected").text() != '') {

            $("#trackid").val($("#track option:selected").val());



            var $track_url = $("#track option:selected").attr('track-url');

            $("#tracklink").attr('href', $track_url);

            $('#l3').css('visibility', 'visible');

            $("#spotify_track_name").val($("#track option:selected").html());
            $("#spotify_track_url").val($track_url);





        }
    });

 
});