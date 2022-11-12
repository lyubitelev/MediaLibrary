import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api/api.service';
import { MediaInfoDto } from '../models/media-info';

@Component({
  selector: 'app-media-list',
  templateUrl: './media-list.component.html',
  styleUrls: ['./media-list.component.css']
})
export class MediaListComponent implements OnInit {
  breakpoint!: number;
  mediaInfos!: MediaInfoDto[];

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.apiService.GerAllMediaAsync().subscribe(mediaInfos => {
      this.mediaInfos = mediaInfos;
    });
    
    this.setBreakpoint();
  }

  onResize(event: any) {
    this.setBreakpoint();
  }

  private setBreakpoint(): void {
    this.breakpoint = (window.innerWidth <= 600) ? 1 : 4;
  }
}
