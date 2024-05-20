$(document).ready(function(){
    console.log("home.js loeaded");
    $('#searchButton').click(function(){
        var invitationCode = $('#invitationCode').val();
        if(invitationCode) {
            $.ajax({
                url: `/slot/${invitationCode}/exists`,
                method: 'GET',
                contentType: 'application/json',
                success: function(data) {
                    $('main').empty();
                    console.log("Slot exists: ", data);
                    var newUrl = `/slot/${invitationCode}`;
                    history.pushState(null, '', newUrl);
                    FavoriteSlots(data);
                },
                error: function(xhr, status, error) {
                    console.error('Error:', error);
                },
                complete: function() {
                }
            });
        } else {
            alert('Please enter an invitation code');
        }
    });
});