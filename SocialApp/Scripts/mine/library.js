﻿function SongModel(json) {
    var self = this;

    self.id = ko.observable(json.id);
    self.title = ko.observable(json.title);
    self.artist = ko.observable(json.artist);
    self.album = ko.observable(json.album);
    self.likes = ko.observable(json.likes);
    self.albumCoverPicturePath = ko.observable(json.albumCoverPicturePath);
    self.duration = ko.observable(json.duration);
    self.fileSizeInMegaBytes = ko.observable(json.fileSizeInMegaBytes);
    self.bitrate = ko.observable(json.bitrate);
    self.filePath = ko.observable(json.filePath);

    self.artistDisplay = ko.computed(function() {
        return self.artist() == null || self.artist() == '' ? 'Unknown' : self.artist();
    });

    self.titleDisplay = ko.computed(function() {
        return self.title() == null || self.artist() == '' ? 'Unknown' : self.title();
    });
    
    self.durationDisplay = ko.computed(function() {
        if (!self.duration()) return null;
        var parts = self.duration().split(':');
        var mapped = parts.filter(function(part) {
            return parseInt(part) != 0;
        }).map(function(x) {
            return parseInt(x);
        });
        var seconds = mapped[mapped.length - 1];
        if (seconds.toString().length == 1) {
            seconds = '0' + seconds;
            mapped[mapped.length - 1] = seconds;
        }
        return mapped.join(':');
    });
    
    self.bitrateDisplay = ko.computed(function() {
        return self.bitrate() + ' kbps';
    });
    
    self.fileSizeInMegaBytesDisplay = ko.computed(function() {
        return self.fileSizeInMegaBytes() + ' MB';
    });
    
    self.downloadUrl = ko.computed(function() {
        return '/song/download/' + self.id();
    });
}

function AppModel() {
    var self = this;

    self.song = ko.observable(new SongModel({}));
    self.songs = ko.observableArray([]);
    self.songsAreEmpty = ko.observable(true);

    window.songs = self.songs; // TODO: REMOVE
    window.song = self.song; // TODO: REMOVE

    self.resetUploadState = function() {
        $('#upload-modal').html($('#browse-song-template').html());
    };

    self.fileSelected = function() {
        var file = document.querySelector('#upload-modal [type=file]').files[0];
        $('#upload-modal .modal-body').fadeOut(function() {
            var modalBody = $(this);
            var progressBarHtml = $('#upload-song-template').html();
            modalBody.html(progressBarHtml).fadeIn();
            var songUploader = new SongUploader(file);
            songUploader.upload(function done() { // TODO: handle errors
                ko.mapping.fromJS(this.uploadedSong, {}, self.song);
                self.songs.push(new SongModel(this.uploadedSong));
                setTimeout(function() { // emulating processing
                    $('#upload-modal').modal('hide');
                    self.resetUploadState();
                    ko.applyBindings(self, $('#upload-modal')[0]);
                    $('#song-edit-modal').modal('show');
                }, 500);
            });
        });
    };

    self.updateSong = function() {
        $.post('/song/update', ko.toJS(self.song)).fail(function(resp) {
            console.error(resp);
        });
    };
    
    self.deleteSong = function() {
        $.post('/song/delete', { id: self.song().id }).done(function(resp) {
            self.songs.remove(self.song());
        });
    };

    self.fetchSongs = function() {
        $.get('/user/uploadedsongs').done(function(songs) {
            ko.utils.arrayForEach(songs, function(song) {
                self.songs.push(new SongModel(song));
            });
        });
    };
    
    self.initializeUi = function() {
        $(".song-list").tooltip({
            selector: '[data-toggle="tooltip"]'
        });
        $('#song-edit-modal').on('hide.bs.modal', function() {
            self.updateSong();
        });
    };
    
    self.editSong = function(song) {
        self.song(song);
        $('#song-edit-modal').modal('show');
    };
    
    self.confirmDelete = function(song) {
        self.song(song);
        $('#song-delete-confirmation').modal('show');
    };
    
    self.noSongsPresent = ko.computed(function() {
        return self.songs().length == 0;
    });

    self.initializeUi();
    self.fetchSongs();
    self.resetUploadState();
}

ko.applyBindings(new AppModel());