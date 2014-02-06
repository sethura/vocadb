/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../KnockoutExtensions/AutoCompleteParams.ts" />
/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="../Repositories/AlbumRepository.ts" />
/// <reference path="../Repositories/SongRepository.ts" />
/// <reference path="ArtistForAlbumEditViewModel.ts" />
/// <reference path="SongInAlbumEditViewModel.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // Edit view model for album.
    export class AlbumEditViewModel {
        
        // Adds a song to the album, by either id (existing song) or name (new song).
        public acceptTrackSelection: (songId: number, songName: string) => void;

		// Adds a new artist to the album
		// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
		// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
		addArtist = (artistId?: number, customArtistName?: string) => {

			if (artistId) {

				this.artistRepository.getOne(artistId, artist => {

					var data: dc.ArtistForAlbumContract = {
						artist: artist,
						isSupport: false,
						name: artist.name,
						id: 0,
						roles: 'Default'
					};

					var link = new ArtistForAlbumEditViewModel(this.repository, data);
					this.artistLinks.push(link);

				});

			} else {
				
				var data: dc.ArtistForAlbumContract = {
					artist: null,
					name: customArtistName,
					isSupport: false,
					id: 0,
					roles: 'Default'
				};

				var link = new ArtistForAlbumEditViewModel(this.repository, data);
				this.artistLinks.push(link);

			}

		};

        // Adds a list of artists (from the track properties view model) to selected tracks.
        public addArtistsToSelectedTracks: () => void;

        // Whether all tracks should be selected.
        public allTracksSelected: KnockoutObservable<boolean>;

        private artistsForTracks: () => dc.ArtistContract[];

		artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;

        // List of artist links for this album.
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;

        // Album disc type.
        public discType: KnockoutObservable<string>;

        // Begins editing properties for multiple tracks. Opens the properties dialog.
        public editMultipleTrackProperties: () => void;

        // Start editing properties for a single song. Opens the properties popup dialog.
        public editTrackProperties: (song: SongInAlbumEditViewModel) => void;

        // State for the song being edited in the properties dialog.
        public editedSong: KnockoutObservable<TrackPropertiesViewModel>;

        // Gets an artist for album link view model by Id.
        public getArtistLink: (artistForAlbumId: number) => ArtistForAlbumEditViewModel;

        // Removes an artist from this album.
        public removeArtist: (artist: ArtistForAlbumEditViewModel) => void;

        // Removes artists (selected from the track properties view model) from selected tracks.
        public removeArtistsFromSelectedTracks: () => void;

        // Removes a track from this album.
        public removeTrack: (song: SongInAlbumEditViewModel) => void;

        // Copies modified state from track properties view model to the single track being edited.
        public saveTrackProperties: () => void;

        public submit = () => {
            this.submitting(true);
            return true;
        };

        public submitting = ko.observable(false);

        // Buttons for the track properties dialog.
        public trackPropertiesDialogButtons: KnockoutObservableArray<any>;

        // Whether the track properties dialog should be visible.
        public trackPropertiesDialogVisible: KnockoutObservable<boolean>;

        // List of tracks for this album.
        public tracks: KnockoutObservableArray<SongInAlbumEditViewModel>;

        // Search parameters for new tracks.
        public trackSearchParams: vdb.knockoutExtensions.AutoCompleteParams;

        // Gets a translated name for an artist role.
        public translateArtistRole: (role: string) => string;

        // Updates track and disc numbers of all tracks for this album. This should be done every time the order changes, or tracks are added or removed.
        private updateTrackNumbers: () => void;

        // List of external links for this album.
        public webLinks: WebLinksEditViewModel;
        
		constructor(public repository: rep.AlbumRepository, songRepository: rep.SongRepository,
			private artistRepository: rep.ArtistRepository,
            artistRoleNames, webLinkCategories: dc.TranslatedEnumField[], data: AlbumEdit) {

			this.artistSearchParams = {
				createNewItem: vdb.resources.albumEdit.addExtraArtist,
				acceptSelection: this.addArtist,
				height: 300
			};

            this.acceptTrackSelection = (songId: number, songName: string) => {

                if (songId) {
                    songRepository.getOne(songId, true, song => {
                        var track = new SongInAlbumEditViewModel({ artists: song.artists, artistString: song.artistString, songAdditionalNames: song.additionalNames, songId: song.id, songName: song.name, discNumber: 1, songInAlbumId: 0, trackNumber: 1 });
                        track.isNextDisc.subscribe(() => this.updateTrackNumbers());
                        this.tracks.push(track);
                    });
                } else {
                    var track = new SongInAlbumEditViewModel({ songName: songName, artists: [], artistString: "", discNumber: 1, songAdditionalNames: "", songId: 0, songInAlbumId: 0, trackNumber: 1 });
                    track.isNextDisc.subscribe(() => this.updateTrackNumbers());
                    this.tracks.push(track);
                }

            };

            this.addArtistsToSelectedTracks = () => {

                _.forEach(_.filter(this.tracks(), s => s.selected()), song => {
                    var added = _.map(_.filter(this.editedSong().artistSelections, a => a.selected() && _.all(song.artists(), a2 => a.artist.id != a2.id)), a3 => a3.artist);
                    song.artists.push.apply(song.artists, added);
                });
                 
                this.trackPropertiesDialogVisible(false);

            };

            this.allTracksSelected = ko.observable(false);

            this.allTracksSelected.subscribe(selected => {
                _.forEach(this.tracks(), s => s.selected(selected));
            });

            this.artistsForTracks = () => {
                var notAllowedTypes = ['Label'];
                return _.map(_.filter(this.artistLinks(), a => a.artist != null && !_.contains(notAllowedTypes, a.artist.artistType)), a => a.artist);
            };

            this.artistLinks = ko.observableArray(_.map(data.artistLinks, artist => new ArtistForAlbumEditViewModel(repository, artist)));

            this.discType = ko.observable(data.discType);

            this.editMultipleTrackProperties = () => {

                var artists = this.artistsForTracks();
                this.editedSong(new TrackPropertiesViewModel(artists, null));
                this.trackPropertiesDialogButtons([
                    { text: "Add to tracks", click: this.addArtistsToSelectedTracks },
                    { text: "Remove from tracks", click: this.removeArtistsFromSelectedTracks }
                ]);
                this.trackPropertiesDialogVisible(true);

            };

            this.editTrackProperties = (song) => {

                var artists = this.artistsForTracks();
                this.editedSong(new TrackPropertiesViewModel(artists, song));
                this.trackPropertiesDialogButtons([{ text: 'Save', click: this.saveTrackProperties }]);
                this.trackPropertiesDialogVisible(true);

            };

            this.editedSong = ko.observable(null);

            this.getArtistLink = (artistForAlbumId) => {
                return _.find(this.artistLinks(), artist => artist.id == artistForAlbumId);
            };

            this.removeArtist = artistForAlbum => {
                this.artistLinks.remove(artistForAlbum);
            };

            this.removeArtistsFromSelectedTracks = () => {

                _.forEach(_.filter(this.tracks(), s => s.selected()), song => {
                    var removed = _.filter(song.artists(), a => _.some(this.editedSong().artistSelections, a2 => a2.selected() && a.id == a2.artist.id));
                    song.artists.removeAll(removed);
                });
                
                this.trackPropertiesDialogVisible(false);

            };

            this.removeTrack = song => {
                this.tracks.remove(song);
            };

            this.saveTrackProperties = () => {

                this.trackPropertiesDialogVisible(false);

                if (this.editedSong) {

                    var selected = _.map(_.filter(this.editedSong().artistSelections, a => a.selected()), a => a.artist);
                    this.editedSong().song.artists(selected);
                    this.editedSong(null);

                }

            };

            this.trackPropertiesDialogButtons = ko.observableArray([{ text: 'Save', click: this.saveTrackProperties }]);

            this.trackPropertiesDialogVisible = ko.observable(false);

            this.tracks = ko.observableArray(_.map(data.tracks, song => new SongInAlbumEditViewModel(song)));

            _.forEach(this.tracks(), song => {
                song.isNextDisc.subscribe(() => this.updateTrackNumbers());
            });

            this.tracks.subscribe(() => this.updateTrackNumbers());

            var songTypes = "Unspecified,Original,Remaster,Remix,Cover,Mashup,Other,Instrumental";
            
            if (data.discType == "Video")
                songTypes += ",MusicPV,DramaPV";

            this.trackSearchParams = {
                acceptSelection: this.acceptTrackSelection,
                createNewItem: "Create new song named '{0}'.", // TODO: localize
                extraQueryParams: { songTypes: songTypes }
            };

            this.translateArtistRole = (role) => {
                return artistRoleNames[role];
            };

            this.updateTrackNumbers = () => {

                var track = 1;
                var disc = 1;

                _.forEach(this.tracks(), song => {

                    if (song.isNextDisc()) {
                        disc++;
                        track = 1;
                    }

                    song.discNumber(disc);
                    song.trackNumber(track);
                    track++;

                });

            };

            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
        }

    }

    // Single artist selection for the track properties dialog.
    export class TrackArtistSelectionViewModel {

        // Whether this artist has been selected.
        selected: KnockoutObservable<boolean>;

        // Whether this selection is visible according to current filter.
        visible: KnockoutComputed<boolean>;

        constructor(public artist: dc.ArtistContract, selected: boolean, filter: KnockoutObservable<string>) {

            this.selected = ko.observable(selected);   

            this.visible = ko.computed(() => {

                var f = filter();
                if (f.length == 0)
                    return true;

                f = f.trim().toLowerCase();

                return (artist.name.toLowerCase().indexOf(f) >= 0 || artist.additionalNames.toLowerCase().indexOf(f) >= 0);

            });

        }
    
    }

    // View model for the track properties dialog, for editing artists for one or more tracks.
    export class TrackPropertiesViewModel {
        
        // Selectable artists.
        artistSelections: TrackArtistSelectionViewModel[];

        // Artist filter string.
        filter: KnockoutObservable<string> = ko.observable("");

        // At least one artist selected for this track.
        somethingSelected: KnockoutComputed<boolean>;

        // At least one artist selectable (not selected and visible).
        somethingSelectable: KnockoutComputed<boolean>;

        constructor(artists: dc.ArtistContract[], public song: SongInAlbumEditViewModel) {
            
            this.artistSelections = _.map(artists, a =>
                new TrackArtistSelectionViewModel(a, song != null && _.some(song.artists(), sa => a.id == sa.id), this.filter));

            this.somethingSelected = ko.computed(() => {
                return _.some(this.artistSelections, a => a.selected());
            });

            this.somethingSelectable = ko.computed(() => {
                return _.some(this.artistSelections, a => !a.selected() && a.visible());
            });
        
        }
    
    }

    export interface AlbumEdit {
        
        artistLinks: dc.ArtistForAlbumContract[];

        discType: string;

        tracks: SongInAlbumEditContract[];

        webLinks: dc.WebLinkContract[];
    
    }

}