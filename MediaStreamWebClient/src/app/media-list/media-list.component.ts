import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api/api.service';
import { MediaInfoDto } from '../models/media-info';
import { Router } from '@angular/router';
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { MediaPlayerDialog } from '../media-player-dialog/media-player.dialog';

@Component({
  selector: 'app-media-list',
  templateUrl: './media-list.component.html',
  styleUrls: ['./media-list.component.css']
})
export class MediaListComponent implements OnInit {
  breakpoint!: number;
  searchText!: string;
  mediaInfos!: Observable<MediaInfoDto[]>;

  constructor(private apiService: ApiService,
    private router: Router,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.search();
    this.setBreakpoint();
  }

  onResize(event: any) {
    this.setBreakpoint();
  }

  play(url: string) {
    this.router.navigate(['play', url]);
  }

  search() {
    this.mediaInfos = this.apiService.GetAllMediaAsync(this.searchText);
  }

  openDialog(id: string, name: string): void {
    const dialogRef = this.dialog.open(MediaPlayerDialog, {
      width: '1200px',
      maxWidth: '100%',
      maxHeight: '100%',
      data: {id: id, name: name},
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  private setBreakpoint(): void {
    if (window.innerWidth >= 1400) {
      this.breakpoint = 4;
      return;
    }

    if (window.innerWidth >= 1300) {
      this.breakpoint = 3;
      return;
    }

    if (window.innerWidth >= 1000 && window.innerHeight <= 800) {
      this.breakpoint = 3;
      return;
    }

    if (window.innerWidth >= 1000 && window.innerHeight <= 600) {
      this.breakpoint = 2;
      return;
    }

    if (window.innerWidth >= 768 && window.innerHeight <= 1368) {
      this.breakpoint = 2;
      return;
    }

    if (window.innerWidth >= 280 && window.innerHeight <= 915) {
      this.breakpoint = 1;
      return;
    }

    this.breakpoint = 4;
  }
}
