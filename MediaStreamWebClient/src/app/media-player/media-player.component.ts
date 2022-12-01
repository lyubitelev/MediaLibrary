import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../api/api.service';

@Component({
  selector: 'app-media-player',
  templateUrl: './media-player.component.html',
  styleUrls: ['./media-player.component.css']
})
export class MediaPlayerComponent implements OnInit {
  fileName!: string;
  url!: string;

  constructor(private apiService: ApiService,
              private activateRoute: ActivatedRoute) { 
    this.fileName = activateRoute.snapshot.params['id'];
  }

  ngOnInit(): void {
    this.url = this.apiService.GetMediaStreamUrl(this.fileName);
  }
}
