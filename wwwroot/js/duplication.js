function duplicateSlot(slotId, userId) {
    var duplicationRequest = {
        primarySlotId: slotId,
        userId: userId
    };
    console.log("duplicateslot(): ", duplicationRequest);
    $.ajax({
        url: "/host/duplicate/slot",
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(duplicationRequest),
        success: function(response) {
            console.log("Slot duplication details:", response);
        },
        error: function(xhr, status, error) {
            console.error("Error duplicating slot:", error);
        }
    });
}