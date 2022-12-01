import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ApiService } from "../api/api.service";

@Component({
  selector: 'media-player.dialog',
  templateUrl: 'media-player.dialog.html',
})
export class MediaPlayerDialog {
  url!: string;

  constructor(private apiService: ApiService,
              public dialogRef: MatDialogRef<MediaPlayerDialog>,
              @Inject(MAT_DIALOG_DATA) public data: any) {
    console.log(data);
    this.url = this.apiService.GetMediaStreamUrl(data.name);
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}