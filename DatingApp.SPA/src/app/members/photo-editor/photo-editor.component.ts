import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from '../../_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import * as _ from 'underscore';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  mainPhoto: Photo;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  public fileOverBase(e: boolean): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.getUserId() + '/photos',
      authToken: 'Bearer ' + this.authService.getToken(),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: Photo = JSON.parse(response);
        this.photos.push(photo);
        // if this is the first photo uploaded by the user
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.getUserId(), photo.id)
      .subscribe(
        () => {
          const oldMain = _.findWhere(this.photos, { isMain: true });
          oldMain.isMain = false;
          photo.isMain = true;
          this.mainPhoto = photo;
          this.authService.changeMemberPhoto(photo.url);
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  deletePhoto(photoId: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService
        .deletePhoto(this.authService.getUserId(), photoId)
        .subscribe(
          () => {
            this.photos.splice(_.findIndex(this.photos, { id: photoId }), 1);
            this.alertify.message('Photo has been deleted');
          },
          error => {
            console.log(error);
            this.alertify.error('Failed to delete photo');
          }
        );
    });
  }
}
