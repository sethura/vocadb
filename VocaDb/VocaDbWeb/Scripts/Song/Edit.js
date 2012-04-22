﻿
function initPage(songId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#statusHelp").tooltip();
	$("#pvLoader")
		.ajaxStart(function () { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: { "Save": function () {

		var artistId = $("#rolesArtistId").val();
		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		var idField = $("#artistsTableBody input.artistId[value='" + artistId + "']");
		var row = idField.parent().parent();
		row.find("input.artistRoles").val(checkedRoles.join(","));
		row.find("div.artistRolesList").html(checkedRoles.join("<br />"));

		$("#editArtistRolesPopup").dialog("close");

	}}});

	$("input.artistRoleCheck").button();

	initNamesList();
	initWebLinksList();

	$("#changeOriginalBtn").click(function () {

		$("#changeOriginalPanel").show();
		return false;

	});

	function acceptOriginalSong(artistId, term) {

		$("#changeOriginalPanel").hide();
		var newOriginalId = $("#changeOriginalId").val();

		$.post("../../Song/CreateSongLink", { songId: newOriginalId }, function (content) {
			$("#originalContent").html(content);
		});

	}

	var acceptNewOriginalBtn = $("#acceptNewOriginalBtn");
	var changeOriginalList = $("#changeOriginalList");
	var changeOriginalName = $("input#changeOriginalName");
	var changeOriginalId = $("#changeOriginalId");

	initEntrySearch(changeOriginalName, changeOriginalList, "Song", "../../Song/FindJsonByName",
		{
			acceptBtnElem: acceptNewOriginalBtn,
			acceptSelection: acceptOriginalSong,
			idElem: changeOriginalId,
			createOptionFirstRow: function (item) { return item.Name; },
			createOptionSecondRow: function (item) { return item.ArtistString; },
			createTitle: function (item) { return item.AdditionalNames; },
			extraQueryParams: {
				ignoredIds: JSON.stringify(songId),
				songTypes: "Unspecified,Original,Remix,Cover,Mashup,Other"
			},
			height: 250
		});

	function acceptArtistSelection(artistId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Song/AddExistingArtist", { songId: songId, artistId: artistId }, artistAdded);
		} else {
			//$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: term }, artistAdded);
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames },
			extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Unknown,Animator,OtherGroup,OtherIndividual" }
		});

	function artistAdded(row) {

		var artistsTable = $("#artistsTableBody");
		artistsTable.append(row);

	}

	$("a.artistRemove").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

	$("a.artistRolesEdit").live("click", function () {

		var row = $(this).parent().parent();

		var id = row.find("input.artistId").val();
		$("#rolesArtistId").val(id);

		var roles = row.find("input.artistRoles").val().split(",");
		$("#editArtistRolesPopup input.artistRoleCheck").each(function () {
			$(this).removeAttr("checked");
			$(this).button("refresh");
		});

		$(roles).each(function () {
			var check = $("#editArtistRolesPopup #artistRole_" + this.trim());
			$(check).attr("checked", "checked");
			$(check).button("refresh");
		});

		$("#editArtistRolesPopup").dialog("open");

		return false;

	});

	$("#pvAdd").click(function () {

		var service = $("#pvService_new").val();
		var pvUrl = $("#pvUrl_new").val();
		var pvType = $("#pvType_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Song/CreatePVForSongByUrl", { songId: songId, pvUrl: pvUrl, type: pvType }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
				return;
			}

			var addRow = $("#pvTableBody");
			addRow.append(response.Result);

		});

		return false;

	});

	$("a.pvRemove").live("click", function () {

		/*var id = getId(this);
		$.post("../../Song/DeletePVForSong", { pvForSongId: id }, function () {

			$("tr#pvRow_" + id).remove();

		});*/

		$(this).parent().parent().remove();
		return false;

	});

	$("#lyricsAdd").click(function () {

		$.post("../../Song/CreateLyrics", null, function (html) {

			$("#lyricsAdd").before(html);

		});

		return false;

	});

	$("a.deleteLyrics").live("click", function () {

		$(this).parent().remove();
		return false;

	});

}