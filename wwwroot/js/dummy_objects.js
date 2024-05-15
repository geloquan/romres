function createDummyRootTrees(invitation_code, slot_name, host_name_tag, root_id) {
    return {
        "slotTrees": [
            {
                "rootId": root_id,
                "invitationCode": invitation_code,
                "secondLayerId": [],
                "thirdLayerId": [],
                "rootSlotModel": {
                    "edge": [],
                    "durations": [],
                    "name": slot_name,
                    "slotId": root_id,
                    "parentSlotId": null,
                    "isReservable": false,
                    "reserverName": "",
                    "invitationCode": invitation_code,
                    "note": "",
                    "hostName": host_name_tag
                },
                "secondLayerChildren": [],
                "thirdLayerChildren": []
            }
        ]
    };
}