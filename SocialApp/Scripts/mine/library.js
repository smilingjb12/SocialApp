﻿function SongModel(json) {
    var self = this;

    self.id = ko.observable(json.id);
    self.title = ko.observable(json.title);
    self.artist = ko.observable(json.artist);
    self.album = ko.observable(json.album);
    self.likes = ko.observable(json.likes);
    self.albumCoverPicturePath = ko.observable(json.albumCoverPicturePath);

    self.artistDisplay = ko.computed(function() {
        return self.artist() == null || self.artist() == '' ? 'Unknown' : self.artist();
    });

    self.titleDisplay = ko.computed(function() {
        return self.title() == null || self.artist() == '' ? 'Unknown' : self.title();
    });
}

function AppModel() {
    var self = this;

    self.song = ko.observable(new SongModel({}));
    self.songs = ko.observableArray([]);

    window.songs = self.songs(); // TODO: REMOVE
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
        $.post('/song/update', ko.toJS(self.song)).done(function(resp) {
            // TODO: do something
            console.log('updated song ', self.song());
            console.log('response: ', resp);
        });
    };
    
    self.deleteSong = function() {
        console.log('removing song', self.song());
        $.post('/song/delete', {id: self.song().id}).done(function(resp) {
            console.log('response: ', resp);
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
        console.log('editing song: ', song);
        self.song(song);
        $('#song-edit-modal').modal('show');
    };
    
    self.confirmDelete = function(song) {
        console.log('confirm delete');
        self.song(song);
        $('#song-delete-confirmation').modal('show');
    };

    self.initializeUi();
    self.fetchSongs();
    self.resetUploadState();
}

ko.applyBindings(new AppModel());