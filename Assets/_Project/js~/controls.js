mergeInto(LibraryManager.library, {
    LoadRoomData: function (onSuccess) {
        let input = document.createElement('input');
        input.type = 'file';
        input.onchange = _ => {
            let reader = new FileReader();
            reader.onload = (event) => {
                let jsonStr = event.target.result;
                let json = JSON.parse(jsonStr);
                console.log(json);
                var bufferSize = lengthBytesUTF8(jsonStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(jsonStr, buffer, bufferSize);
                dynCall_vi(onSuccess, buffer);
                _free(buffer);
            };
            reader.readAsText(input.files[0]);
        };
        input.click();
    },

    SaveRoomData: function (roomData) {
        var strData = UTF8ToString(roomData);
        const a = document.createElement('a');
        const file = new Blob([strData], { type: "application/json" });
        a.href = URL.createObjectURL(file);
        a.download = "Room Data.json";
        a.click();
        URL.revokeObjectURL(a.href);
    },

    PrintRoomData: function (roomData) {
        var strData = UTF8ToString(roomData);
        const a = document.createElement('a');
        const file = new Blob([strData], { type: "text/plain" });
        a.href = URL.createObjectURL(file);
        a.download = "List Of Items.txt";
        a.click();
        URL.revokeObjectURL(a.href);
    }

    //,
    //GetRoomData: function (onSuccess) {
    //    fetch('https://api.npoint.io/70d3c4a116b984c81f12', {
    //        method: 'GET',
    //        headers: {
    //            'Content-Type': 'application/json'
    //        }
    //    })
    //        .then((response) => response.json())
    //        .then((json) => {
    //            console.log(json);
    //            var jsonStr = JSON.stringify(json);
    //            var bufferSize = lengthBytesUTF8(jsonStr) + 1;
    //            var buffer = _malloc(bufferSize);
    //            stringToUTF8(jsonStr, buffer, bufferSize);
    //            dynCall_vi(onSuccess, buffer);
    //            _free(buffer);
    //        });
    //}
});